﻿using System;
using UnityEngine;

namespace RollABall
{
    public class GameController : MonoBehaviour, IDisposable
    {
        private ListExecuteObject _interactiveObject;
        private Data data;
        private PlayerBall player;
        private CameraController cameraController;
        private MiniMapController miniMapController;
        private InputController inputController;
        private BonusDisplay _bonusDisplay;
        private GameOverDisplay _gameOverDisplay;
        private InteractiveObject[] _bonuses;

        private void Awake()
        {
            _interactiveObject = new ListExecuteObject();
            data = new Data();
            player = data.PlayerBall;
            _bonuses = FindObjectsOfType<InteractiveObject>();
            cameraController = new CameraController(player.transform, data.MainCamera.transform);
            inputController = new InputController(player, _bonuses);
            miniMapController = new MiniMapController(player.transform, data.MiniMapCamera.transform);
            _interactiveObject.AddExecuteObject(cameraController);
            _interactiveObject.AddExecuteObject(inputController);
            _bonusDisplay = new BonusDisplay(data.Bonus);
            _gameOverDisplay = new GameOverDisplay(data.EndGame);

            player.ShowScore += _bonusDisplay.Display;


            foreach (var o in _interactiveObject)
            {
                if (o is GoodBonus goodBonus)
                {
                    goodBonus.ShakeCamera += cameraController.SetShakeDuration;
                    goodBonus.SetBonusPoint += player.SetBonusPoint;
                }
                if (o is SpeedBonus speedBonus)
                {
                    speedBonus.ShakeCamera += cameraController.SetShakeDuration;
                    speedBonus.SetBonusSpeedPoint += player.SetBonusSpeedPoint;
                }
                if (o is BadBonus badBonus)
                {
                    badBonus.ShowGameOverLabel += _gameOverDisplay.GameOver;
                }
            }

        }

        private void Update()
        {
            for (var i = 0; i < _interactiveObject.Length; i++)
            {
                var interactiveObject = _interactiveObject[i];

                if (interactiveObject == null)
                {
                    continue;
                }
                interactiveObject.Execute();
            }
        }

        private void LateUpdate()
        {
            miniMapController.LateExecute();
        }

        public void Dispose()
        {
            foreach (var o in _interactiveObject)
            {
                if (o is GoodBonus goodBonus)
                {
                    goodBonus.ShakeCamera -= cameraController.SetShakeDuration;
                    goodBonus.SetBonusPoint -= player.SetBonusPoint;
                }
                if (o is SpeedBonus speedBonus)
                {
                    speedBonus.ShakeCamera -= cameraController.SetShakeDuration;
                    speedBonus.SetBonusSpeedPoint -= player.SetBonusSpeedPoint;
                }
                if (o is BadBonus badBonus)
                {
                    badBonus.ShowGameOverLabel -= _gameOverDisplay.GameOver;
                }
            }
            player.ShowScore -= _bonusDisplay.Display;
        }
    }
}
