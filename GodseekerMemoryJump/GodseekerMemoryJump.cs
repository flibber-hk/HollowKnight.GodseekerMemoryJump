using System.Collections;
using System.Linq;
using HutongGames.PlayMaker;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vasi;

namespace GodseekerMemoryJump
{
    public class GodseekerMemoryJump : Mod, ITogglableMod
    {
        internal static GodseekerMemoryJump instance;
        public bool ModEnabled = false;

        public GodseekerMemoryJump() : base("Godseeker Memory Jump")
        {
            instance = this;
        }
        public override string GetVersion()
        {
            return "1.1";
        }

        public override void Initialize()
        {
            Log("Initializing Mod...");
            
            ModEnabled = true;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += EnableGodseekerMemoryJump;
        }
        public void Unload()
        {
            ModEnabled = false;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= EnableGodseekerMemoryJump;
        }

        private void EnableGodseekerMemoryJump(Scene scene, LoadSceneMode lsm)
        {
            if (scene.name != "GG_Waterways") return;

            GameManager.instance.StartCoroutine(PatchFsms(scene));
        }

        private IEnumerator PatchFsms(Scene scene)
        {
            GameObject godseeker = scene.GetRootGameObjects().FirstOrDefault(obj => obj.name == "Godseeker Waterways");
            if (godseeker == null) yield break;

            yield return null;
            yield return null;
            yield return null;

            foreach (PlayMakerFSM fsm in godseeker.GetComponentsInChildren<PlayMakerFSM>(true))
            {
                if (fsm.gameObject.name == "Dream Enter" && fsm.FsmName == "Control")
                {
                    FsmState state = fsm.Fsm.GetState("Wastes Cutscene?");
                    state.InsertMethod(0, () =>
                    {
                        if (HeroController.instance.cState.facingRight && ModEnabled)
                        {
                            fsm.SendEvent("CUTSCENE");
                        }
                    });
                }
            }
        }
    }
}