using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

// Thank you: #Code_Modding, SnipUnderCover, Brokemia, Kalobi, Kuksattu, USSRNAME, and many others who helped.

namespace Celeste.Mod.CyrusSandbox.Entities
{
    [CustomEntity("CyrusSandbox/InteractiveEntity")]
    public class InteractiveEntity : NPC
    {

        public TalkComponent talker;

        private readonly Sprite mainSprite = GFX.SpriteBank.Create("InteractiveEntity");
        private readonly Sprite playerAnims = GFX.SpriteBank.Create("PlayerInteract");

        private string plrAnim;
        private string entityAnim;
        private string entityTalkAnim;

        public string flag;
        public bool removeHair;
        public bool fromRight;
        public float talkDuration;
        public string petSound;
        public string petEndSound;
        public float playerOffset;
        public bool indicator;

        private Coroutine petCoro;

        public InteractiveEntity(EntityData data, Vector2 position)
        : base(data.Position + position)
        {

            Collider = new Hitbox(16, 8, 0, 0);
            Depth = -1;

            flag = data.Attr("flag", "");
            talkDuration = data.Float("TalkDuration", 2f);
            removeHair = data.Bool("MakeHairInvisible", false);
            playerOffset = data.Float("Spacing", 0f);
            indicator = data.Bool("ShowIndicator", true);
            entityTalkAnim = data.Attr("talkAnimation", "sunnyPetting");
            entityAnim = data.Attr("idleAnimation", "sunnyIdle");
            plrAnim = data.Attr("playerAnimation", "plrPetting");
            petSound = data.Attr("TalkSound", "");
            petEndSound = data.Attr("TalkEndSound");

            mainSprite.Play(entityAnim);
            Add(mainSprite);

            Add(talker = new TalkComponent(new Rectangle(-25, 0, 64, 8), indicator == false ? new Vector2(0f, -99999f) : new Vector2(2f, -4f), Interact));

            position = Position;

        }

        private void Interact(Player player)
        {
            Add(petCoro = new Coroutine(Petting(player)));
        }

        private IEnumerator Petting(Player player)
        {
            yield return PlayerApproach(player, turnToFace: true, playerOffset);
            player.Facing = (playerOffset > 0 ? Facings.Left : Facings.Right);
            mainSprite.Play(entityTalkAnim);
            player.Add(playerAnims);
            playerAnims.Justify = new Vector2(0.5f, 1f);
            playerAnims.Play(plrAnim);
            if (removeHair) { player.Hair.Visible = false; }
            player.Sprite.Visible = false;
            mainSprite.Visible = true;
            Audio.Play(petSound);
            yield return talkDuration;
            player.Sprite.Visible = true;
            player.Hair.Visible = true;
            player.Remove(playerAnims);
            mainSprite.Play(entityAnim);
            Audio.Play(petEndSound);
            Level.EndCutscene();
            SceneAs<Level>().Session.SetFlag(flag);
            player.StateMachine.Locked = false;
            player.StateMachine.state = 0;
        }

    }
}
