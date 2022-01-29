using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Chess.Core;
using Chess.UI;
using Mirror;
using System;

namespace Chess.Combat {
    public class Unit : NetworkBehaviour {
        [SyncVar] public Player player;
        public float HealthPercentage => currHealth / maxHealth;

        [SerializeField] MovementPattern[] movementPatterns;
        [SerializeField] List<Ability> abilities; //movement and attacking will be considered abilities
        [SerializeField] int maxActionPoints;
        [SerializeField] float maxHealth;


        [SyncVar] public int currActionPoints;
        [SyncVar] public float currHealth;
        [SyncVar] private Tile currTile;
        private Queue<MovementPattern> patternQueue;
        private PlayerType playerType => player.playerPointer.playerType;
        private IsSelectable selectable;
        private OnTile onTile;

        private Ability activeAbility;

        #region Server
        [Server]
        public void Move(Tile newTile) {
            currTile?.AddOnTile(null);
            currTile = newTile;
            transform.position = currTile.transform.position;
            currTile.AddOnTile(onTile);
        }

        [Server]
        public void TakeDamage(float damage) {
            print(name + " took " + damage + " damage");
            currHealth -= damage;
            if (currHealth <= 0) {
                Die();
            }
        }

        [Server]
        private void Die() {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client
        public void Awake() {
            //print("start");
            SetQueue();
            //playerPointer = FindObjectsOfType<PlayerPointer>().Where(x => x.playerType == playerType).First();
            //playerType = playerPointer.playerType;
            selectable = GetComponent<IsSelectable>();
            selectable.AddSelectionValidParameters(IsSelectable);
            selectable.AddOnSelectEvent(OnSelect);
            selectable.AddOnDeselectEvent(OnDeselect);
            onTile = GetComponent<OnTile>();
            currActionPoints = maxActionPoints;
            currHealth = maxHealth;
            if (GetAbility<AMove>() == null) Debug.LogError("no move ability on " + name);
            if (GetAbility<A_Attack>() == null) Debug.LogError("no attack ability on " + name);
            //     not able to move units and units are able to be spawned on top of each other. I think that OnTile is not getting set.
        }


        [Command]
        public void CmdMove(Tile newTile) {
            Move(newTile);
        }

        [Command]
        public void CmdDealDamage(float damageAmount, Unit targetUnit) {
            targetUnit.TakeDamage(damageAmount);
        }

        [Command]
        public void CmdTakeDamage(float damageAmount) {
            TakeDamage(damageAmount);
        }

        [Command]
        public void CmdDecreaseActionPoints(int amount) {
            currActionPoints -= amount;
            print(name + " ap is now " + currActionPoints);
        }
        #endregion

        public bool IsSelectable(PlayerType playerType) => this.playerType == playerType;

        public void OnSelect() => GlobalPointers.UI_Manager.SetUI(UIType.abilityDisplayManager, this);

        public void OnDeselect() {
            //GenerateValidMovements(currentPattern);
            activeAbility?.CancelAbility();
        }

        private void UpdateActionPointDisplay() {

        }

        public Tile GetCurrentTile() => currTile;

        public MovementPattern GetNextPattern() {
            var temp = patternQueue.Dequeue();
            patternQueue.Enqueue(temp);
            return temp;
        }
        private void SetQueue() {
            System.Random rnd = new System.Random();
            patternQueue = new Queue<MovementPattern>(movementPatterns.OrderBy(pat => rnd.Next()));
        }
        
        public void SetActiveAbility(Ability ability) {
            activeAbility = ability;
        }

        public List<Ability> GetAbilities() => abilities;

        public t GetAbility<t>() where t:Ability {
            return GetAbilities().Find(ability => ability is t) as t;
            }
    }
}