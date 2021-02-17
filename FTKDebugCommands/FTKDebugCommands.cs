using MelonLoader;
using Harmony;

namespace FTKDebugCommands
{
    public class FTKDebugCommands : MelonMod
    {
        public override void OnApplicationStart()
        {
            var harmony = HarmonyInstance.Create("com.ftk.debugcommands");
            harmony.PatchAll();
        }

        public override void OnLevelWasLoaded(int level)
        {
            if (level == 1)
                FTKVersion.Instance.m_VersionNum = FTKVersion.Instance.m_VersionNum + ".modded";
        }

    }
    [HarmonyPatch(typeof(SerializeGO), "ShowBugForm")]
    class NukeShowBugForm
    {
        static bool Prefix()
        {
            MelonLogger.Log("Mods are loaded - do not report bugs to IronOak Games. Please report all bugs to the mod developer. Nikm#0482 in the official FTK Discord server.");
            return false;
        }
    }

    [HarmonyPatch(typeof(uiChatBox), "OnInputFinished")]
    class OnInputFinsihedPatch
    {
        static bool Prefix(string _s)
        {
            if (_s == "/partyheal")
            {
                EncounterSession.Instance.OverworldFullPartyHeal();
                EncounterSession.Instance.CombatFullPartyHeal();
                foreach (CharacterOverworld characterOverworld in FTKHub.Instance.m_CharacterOverworlds)
                {
                    if (!characterOverworld.m_WaitForRespawn)
                    {
                        characterOverworld.m_CharacterStats.PlayerFullHealCheat();
                    }
                }
                uiChatBox.Instance.m_InputBox.text = string.Empty;
                return false;
            }
            else if(_s.StartsWith("/give"))
            {
                string itemname = _s.Substring(6).ToLower();
                CharacterOverworld cow = GameLogic.Instance.GetCurrentCOW();
                int i = 0;
                foreach (Google2u.TextItemsRow row in Google2u.TextItems.Instance.Rows)
                {
                    if (row._en.ToString().ToLower().Equals(itemname))
                    {
                        string baseitemname = Google2u.TextItems.Instance.rowNames[i].Substring(4);
                        cow.AddItemToBackpack(GridEditor.FTK_itembase.GetEnum(baseitemname), cow);
                    }
                    i += 1;
                }
                uiChatBox.Instance.m_InputBox.text = string.Empty;
                return false;
            }
            else if(_s == "/reveal")
            {
                GameLogic.Instance.RevealMap();
                uiChatBox.Instance.m_InputBox.text = string.Empty;
                return false;
            }
            else if (_s == "/unreveal")
            {
                GameLogic.Instance.UnrevealMap();
                uiChatBox.Instance.m_InputBox.text = string.Empty;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
