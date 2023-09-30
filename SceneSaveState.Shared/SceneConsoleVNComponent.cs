using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.VNDataComponent;
using static SceneSaveState.VNDataComponent.VNData;

namespace SceneSaveState
{
    internal class SceneConsoleVNComponent
    {
        internal const string defaultSpeakerAlias = "s";
        private VNData currentVNData;
        private VNController gameController;
        private RoleTracker roleTracker;
        public SceneConsoleVNComponent(VNController c)
        {
            this.gameController = c;
            currentVNData = new VNData
            {
                enabled = false,
                whosay = "",
                whatsay = "",
                addvncmds = "",
                addprops = new addprops_struct()
            };

            currentVNData.addprops.a1 = false;
            currentVNData.addprops.a2 = false;
        }

        internal string get_next_speaker(Dictionary<string, int> allActors,  string curSpeakAlias, bool next)
        {
            // next from unknown speaker
            var keylist = allActors.Keys.ToList();
            if (curSpeakAlias != defaultSpeakerAlias && !allActors.ContainsKey(curSpeakAlias))
                return defaultSpeakerAlias;
            // next from s or actor
            if (curSpeakAlias == defaultSpeakerAlias)
            {
                if (allActors.Count > 0)
                {
                    if (next)
                        return keylist[0];
                    return keylist.Last();
                }

                return defaultSpeakerAlias;
            }

            var nextIndex = keylist.IndexOf(curSpeakAlias);
            if (next)
                nextIndex += 1;
            else
                nextIndex -= 1;
            return Enumerable.Range(0, allActors.Count).Contains(nextIndex) ? keylist[nextIndex] : defaultSpeakerAlias;
        }

        internal void DrawVNDataOptions(View v)
        {
            currentVNData.enabled = GUILayout.Toggle(currentVNData.enabled, "Use cam in Visual Novel");
            if (!currentVNData.enabled) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Who say:", GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                currentVNData.whosay = get_next_speaker(roleTracker.CharacterRoles, currentVNData.whosay, false);
            }
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                currentVNData.whosay = get_next_speaker(roleTracker.CharacterRoles, currentVNData.whosay, true);
            }
            GUILayout.EndHorizontal();
            currentVNData.whosay = GUILayout.TextField(currentVNData.whosay);

            GUILayout.BeginHorizontal();
            GUILayout.Label("What say:", GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                currentVNData.whatsay = "";
            }
            if (GUILayout.Button("...", GUILayout.Width(20)))
            {
                currentVNData.whatsay = "...";
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            currentVNData.whatsay = GUILayout.TextArea(currentVNData.whatsay, GUILayout.Height(85));
            if (GUILayout.Button("Save", GUILayout.Width(20)))
            {
                v.Update(currentVNData);
            }
            GUILayout.EndVertical();

            /*GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                Instance.currentVNData.addvncmds = GUILayout.TextArea(Instance.currentVNData.addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.currentVNData.addvncmds = "";
                }
                // if GUILayout.Button("X", GUILayout.Width(20)):
                //     sc.cam_whatsay = ""
                // if GUILayout.Button("...", GUILayout.Width(20)):
                //     sc.cam_whatsay = "..."
                GUILayout.EndHorizontal();
                */
        }

        internal void SetVNData(VNData vnData)
        {
            currentVNData.enabled = vnData.enabled;
            currentVNData.whosay = vnData.whosay is null ? "" : vnData.whosay;
            currentVNData.whatsay = vnData.whatsay is null ? "" : vnData.whatsay;
            if (vnData.addvncmds != null)
                currentVNData.addvncmds = vnData.addvncmds;
            else
                currentVNData.addvncmds = "";

            currentVNData.addprops = vnData.addprops;

            gameController.SetText(currentVNData.whosay, currentVNData.whatsay);
        }

        internal void ResetVNData()
        {
            currentVNData.enabled = false;
            currentVNData.whosay = "";
            currentVNData.whatsay = "";
            currentVNData.addvncmds = "";
            currentVNData.addprops.a1 = false;
            currentVNData.addprops.a2 = false;
        }

    }
}
