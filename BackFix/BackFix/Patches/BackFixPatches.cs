using API;
using Enemies;
using HarmonyLib;
using UnityEngine;

namespace BackFix.Patches {
    internal class KnifeFixPatch {

        [HarmonyPatch(typeof(Dam_EnemyDamageLimb), nameof(Dam_EnemyDamageLimb.ApplyDamageFromBehindBonus))]
        [HarmonyPrefix]
        private static bool Postfix_DamageFromBehind(Dam_EnemyDamageLimb __instance, ref float __result, float dam, Vector3 pos, Vector3 dir, float backstabberMulti) {
            if (__instance.m_base.Owner.EnemyBalancingData.AllowDamgeBonusFromBehind) {
                Vector3 rhs = __instance.m_base.Owner.Forward;
                float angle = Vector3.Dot(dir, rhs);
                float num2 = angle;
                num2 = Mathf.Clamp01(num2 + 0.25f) + 1f;
                APILogger.Debug($"back: {angle} {num2}");
                dam *= num2;
                if (angle > 0.55 && backstabberMulti > 1f) {
                    APILogger.Debug($"Applied backstabber multi!");
                    dam *= backstabberMulti;
                }
            }
            __result = dam;
            return false;
        }
    }

    internal class BackFixPatches {
        [HarmonyPatch(typeof(EnemySync), nameof(EnemySync.OnSpawn))]
        [HarmonyPostfix]
        private static void OnSpawn(EnemySync __instance) {
            if (!__instance.m_agent.gameObject.GetComponent<SyncPosition>()) {
                SyncPosition sync = __instance.m_agent.gameObject.AddComponent<SyncPosition>();
                sync.enemy = __instance.m_agent;
            }
        }

        internal class SyncPosition : MonoBehaviour {
            public EnemyAgent? enemy;

            private void Update() {
                if (enemy == null) return;
                enemy.Position = enemy.transform.position;
                enemy.Forward = enemy.transform.forward;
            }
        }
    }
}
