using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Yendor.Tests
{
    public sealed class CombatBehaviorTests
    {
        GameObject playerObject, enemyObject, cover;
        PlayerMotor player;
        Enemy enemy;
        Health life;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
            playerObject = new GameObject("Combat target");
            playerObject.transform.position = new Vector3(2, 1000, 0);
            player = playerObject.AddComponent<PlayerMotor>(); player.enabled = false;
            life = player.GetComponent<Health>();
            enemyObject = new GameObject("Combat attacker");
            enemyObject.transform.position = new Vector3(0, 1000, 0);
            enemy = enemyObject.AddComponent<Enemy>(); enemy.enabled = false;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (cover) Object.DestroyImmediate(cover);
            if (playerObject) Object.DestroyImmediate(playerObject);
            if (enemyObject) Object.DestroyImmediate(enemyObject);
            yield return new ExitPlayMode();
        }
        void MoveTarget(Vector3 position)
        {
            var motor = player.GetComponent<CharacterController>();
            motor.enabled = false; player.transform.position = position; motor.enabled = true;
            Physics.SyncTransforms();
        }
        void Configure(EnemyRole role, float distance)
        {
            enemy.Configure(role, 0);
            MoveTarget(new Vector3(distance, 1000, 0));
        }

        [UnityTest]
        public IEnumerator BruteWarnsThenHitsOnceAndAllowsDodging()
        {
            Configure(EnemyRole.Brute, 2);
            enemy.TickCombat(10, 0, player, false);
            Assert.That(enemy.WindingUp, Is.True);
            Assert.That(life.Current, Is.EqualTo(100), "Windup must not damage");
            enemy.TickCombat(10.4f, 0, player, false);
            Assert.That(life.Current, Is.EqualTo(100));
            enemy.TickCombat(11, 0, player, false);
            Assert.That(life.Current, Is.LessThan(100), "Stationary target must be hit after warning");
            float after = life.Current;
            enemy.TickCombat(11.1f, 0, player, false);
            Assert.That(life.Current, Is.EqualTo(after), "Recovery prevents repeated strikes");
            enemy.TickCombat(13, 0, player, false);
            MoveTarget(new Vector3(5, 1000, 0));
            enemy.TickCombat(14, 0, player, false);
            Assert.That(life.Current, Is.EqualTo(after), "Leaving committed strike must dodge");
            yield break;
        }
        [UnityTest]
        public IEnumerator SpitterAimIsCommittedAndCoverAtReleaseStopsDamage()
        {
            Configure(EnemyRole.Spitter, 10);
            enemy.TickCombat(10, 0, player, false);
            Assert.That(enemy.WindingUp, Is.True);
            Vector3 aim = enemy.AimPoint;
            MoveTarget(new Vector3(10, 1000, 3));
            Assert.That(enemy.AimPoint, Is.EqualTo(aim));
            enemy.TickCombat(12, 0, player, false);
            Assert.That(life.Current, Is.EqualTo(100), "Aimed shot must miss a dodging target");
            MoveTarget(new Vector3(10, 1000, 0));
            enemy.TickCombat(15, 0, player, false);
            cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cover.transform.position = new Vector3(5, 1001, 0); cover.transform.localScale = new Vector3(1, 4, 4);
            Physics.SyncTransforms();
            Assert.That(enemy.HasClearShot(player, enemy.AimPoint), Is.False);
            enemy.TickCombat(17, 0, player, false);
            Assert.That(life.Current, Is.EqualTo(100), "Cover placed during warning must stop release");
            cover.GetComponent<Collider>().enabled = false; Physics.SyncTransforms();
            enemy.TickCombat(20, 0, player, false);
            enemy.TickCombat(22, 0, player, false);
            Assert.That(life.Current, Is.LessThan(100), "Clear stationary target must be hit");
            yield break;
        }
        [UnityTest]
        public IEnumerator FlankerApproachesAlongAnOffsetRouteThenWarns()
        {
            Configure(EnemyRole.Flanker, 12);
            Vector3 before = enemy.transform.position;
            enemy.TickCombat(10, .2f, player, false);
            Assert.That(enemy.transform.position.x, Is.GreaterThan(before.x));
            Assert.That(Mathf.Abs(enemy.transform.position.z - before.z), Is.GreaterThan(.2f), "Must flank rather than pursue a straight line");
            MoveTarget(enemy.transform.position + Vector3.right * 2);
            enemy.TickCombat(11, 0, player, false);
            Assert.That(enemy.WindingUp, Is.True);
            Assert.That(life.Current, Is.EqualTo(100));
            enemy.TickCombat(12, 0, player, false);
            Assert.That(life.Current, Is.LessThan(100));
            yield break;
        }
        [UnityTest]
        public IEnumerator EndOrDeathCancelsPendingAttacks()
        {
            Configure(EnemyRole.Brute, 2);
            enemy.TickCombat(10, 0, player, false);
            enemy.TickCombat(12, 0, player, true);
            Assert.That(enemy.WindingUp, Is.False);
            Assert.That(life.Current, Is.EqualTo(100));
            enemy.TickCombat(13, 0, player, false);
            int deaths = 0;
            var enemyLife = enemy.GetComponent<Health>(); enemyLife.Died += () => deaths++;
            enemyLife.Damage(1000, Element.Kinetic);
            enemyLife.Damage(1000, Element.Kinetic);
            enemy.TickCombat(15, 0, player, false);
            Assert.That(enemy.WindingUp, Is.False);
            Assert.That(life.Current, Is.EqualTo(100));
            Assert.That(deaths, Is.EqualTo(1));
            yield break;
        }
    }
}
