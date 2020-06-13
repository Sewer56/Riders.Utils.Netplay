﻿using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Riders.Netplay.Messages.Reliable.Structs.Menu.Commands;
using Riders.Tweakbox.Misc;
using Sewer56.SonicRiders;
using Sewer56.SonicRiders.API;
using Sewer56.SonicRiders.Structures.Tasks;
using Sewer56.SonicRiders.Structures.Tasks.Base;
using Sewer56.SonicRiders.Structures.Tasks.Enums.States;
using Sewer56.SonicRiders.Utility;

namespace Riders.Tweakbox.Controllers
{
    public unsafe class EventController : TaskEvents
    {
        /// <summary>
        /// Executed when the user exits the character select menu.
        /// </summary>
        public event AsmAction OnExitCharaSelect;

        /// <summary>
        /// Queries the user whether the character select menu should be left.
        /// </summary>
        public event AsmFunc OnCheckIfExitCharaSelect;

        /// <summary>
        /// Executed when the Enter key is pressed to start a race in character select.
        /// </summary>
        public event AsmAction OnStartRace;

        /// <summary>
        /// Queries the user whether the race should be started.
        /// </summary>
        public event AsmFunc OnCheckIfStartRace;

        /// <summary>
        /// Executed when the stage intro is skipped.
        /// </summary>
        public event AsmAction OnRaceSkipIntro;

        /// <summary>
        /// Queries the user whether the intro should be skipped.
        /// </summary>
        public event AsmFunc OnCheckIfSkipIntro;

        /// <summary>
        /// Provides a "last-chance" event to modify stage load properties, such as the number of players
        /// or cameras to be displayed after stage load. Consider some fields in the <see cref="State"/> class.
        /// </summary>
        public event SetupRace OnSetupRace;

        private RuleSettingsLoop _rule = new RuleSettingsLoop();
        private CourseSelectLoop _course = new CourseSelectLoop();

        private IAsmHook _onExitCharaSelectHook;
        private IAsmHook _onCheckIfExitCharaSelectHook;
        private IAsmHook _onStartRaceHook;
        private IAsmHook _onCheckIfStartRaceHook;
        private IAsmHook _skipIntroCameraHook;
        private IAsmHook _checkIfSkipIntroCamera;
        private IAsmHook _onSetupRaceSettingsHook;

        public EventController()
        {
            var utilities = SDK.ReloadedHooks.Utilities;

            var onExitCharaSelectAsm = new[] { $"use32\n{AsmHelpers.AssembleAbsoluteCall(OnExitCharaSelectHook, out _)}" };
            var ifExitCharaSelectAsm = new string[] { utilities.GetAbsoluteJumpMnemonics((IntPtr)0x00463741, Environment.Is64BitProcess) };
            var onCheckIfExitCharaSelectAsm = new[] { $"use32\n{AsmHelpers.AssembleAbsoluteCall(OnCheckIfExitCharaSelectHook, out _, ifExitCharaSelectAsm, null, null, "je")}" };

            var onStartRaceAsm = new[] { $"use32\n{AsmHelpers.AssembleAbsoluteCall(OnStartRaceHook, out _)}" };
            var ifStartRaceAsm = new string[] { utilities.GetAbsoluteJumpMnemonics((IntPtr)0x0046364B, Environment.Is64BitProcess) };
            var onCheckIfStartRaceAsm = new[] { $"use32\n{AsmHelpers.AssembleAbsoluteCall(OnCheckIfStartRaceHook, out _, ifStartRaceAsm, null, null, "je")}" };

            var onSkipIntroAsm = new[] { $"use32\n{AsmHelpers.AssembleAbsoluteCall(OnSkipIntroHook, out _)}" };
            var ifSkipIntroAsm = new string[] { utilities.GetAbsoluteJumpMnemonics((IntPtr)0x00415F8E, Environment.Is64BitProcess) };
            var onCheckIfSkipIntroAsm = new[] { $"use32\n{AsmHelpers.AssembleAbsoluteCall(OnCheckIfSkipIntroHook, out _, ifSkipIntroAsm, null, null, "je")}" };

            var hooks = SDK.ReloadedHooks;
            _onExitCharaSelectHook = hooks.CreateAsmHook(onExitCharaSelectAsm, 0x00463741, AsmHookBehaviour.ExecuteFirst).Activate();
            _onCheckIfExitCharaSelectHook = hooks.CreateAsmHook(onCheckIfExitCharaSelectAsm, 0x00463732, AsmHookBehaviour.ExecuteFirst).Activate();
            _skipIntroCameraHook = hooks.CreateAsmHook(onSkipIntroAsm, 0x00416001, AsmHookBehaviour.ExecuteFirst).Activate();
            _checkIfSkipIntroCamera = hooks.CreateAsmHook(onCheckIfSkipIntroAsm, 0x415F2F, AsmHookBehaviour.ExecuteFirst).Activate();
            _onStartRaceHook = hooks.CreateAsmHook(onStartRaceAsm, 0x0046364B, AsmHookBehaviour.ExecuteFirst).Activate();
            _onCheckIfStartRaceHook = hooks.CreateAsmHook(onCheckIfStartRaceAsm, 0x0046352B, AsmHookBehaviour.ExecuteFirst).Activate();

            _onSetupRaceSettingsHook = hooks.CreateAsmHook(new[]
            {
                $"use32",
                $"{AsmHelpers.AssembleAbsoluteCall(() => OnSetupRace?.Invoke((Task<TitleSequence, TitleSequenceTaskState>*) (*State.CurrentTask)), out _)}"
            }, 0x0046C139, AsmHookBehaviour.ExecuteFirst).Activate();
        }

        /// <summary>
        /// Disables the event tracker.
        /// </summary>
        public new void Disable()
        {
            base.Disable();
            _onExitCharaSelectHook.Disable();
            _onCheckIfExitCharaSelectHook.Disable();
            _onStartRaceHook.Disable();
            _onCheckIfStartRaceHook.Disable();
            _skipIntroCameraHook.Disable();
            _checkIfSkipIntroCamera.Disable();
            _onSetupRaceSettingsHook.Disable();
        }

        /// <summary>
        /// Re-enables the event tracker.
        /// </summary>
        public new void Enable()
        {
            base.Enable();
            _onExitCharaSelectHook.Enable();
            _onCheckIfExitCharaSelectHook.Enable();
            _onStartRaceHook.Enable();
            _onCheckIfStartRaceHook.Enable();
            _skipIntroCameraHook.Enable();
            _checkIfSkipIntroCamera.Enable();
            _onSetupRaceSettingsHook.Enable();
        }

        private void OnExitCharaSelectHook() => OnExitCharaSelect?.Invoke();
        private bool OnCheckIfExitCharaSelectHook() => OnCheckIfExitCharaSelect != null && OnCheckIfExitCharaSelect.Invoke();

        private void OnStartRaceHook() => OnStartRace?.Invoke();
        private bool OnCheckIfStartRaceHook() => OnCheckIfStartRace != null && OnCheckIfStartRace.Invoke();

        private void OnSkipIntroHook() => OnRaceSkipIntro?.Invoke();
        private bool OnCheckIfSkipIntroHook() => OnCheckIfSkipIntro != null && OnCheckIfSkipIntro.Invoke();

        public delegate void SetupRace(Task<TitleSequence, TitleSequenceTaskState>* task);
        public unsafe delegate void CourseSelectUpdated(CourseSelectLoop loop, Task<CourseSelect, CourseSelectTaskState>* task);
        public unsafe delegate void RuleSettingsUpdated(RuleSettingsLoop loop, Task<RaceRules, RaceRulesTaskState>* task);
    }
}
