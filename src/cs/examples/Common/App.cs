// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.Reflection;

namespace Common;

public sealed unsafe class App : IDisposable
{
    private int _exampleIndex = -1;
    private int _goToExampleIndex;
    private int _examplesCount;
    private ImmutableArray<Type> _exampleTypes = [];
    private ExampleBase? _currentExample;

    public int Run()
    {
        return !Initialize() ? 1 : Loop();
    }

    public void Dispose()
    {
        _currentExample?.QuitInternal();
        _currentExample = null;
    }

    private bool Initialize()
    {
        if (!SDL_Init(SDL_INIT_VIDEO | SDL_INIT_GAMEPAD))
        {
            Console.Error.WriteLine("Failed to initialize SDL: " + SDL_GetError());
            return false;
        }

        var assemblyName = Assembly.GetEntryAssembly()!.GetName().Name;
        Console.WriteLine($"Welcome to the {assemblyName} example suite!");
        Console.WriteLine("Press 1/2 (or LB/RB) to move between examples!");

        _exampleTypes = [
            ..AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ExampleBase).IsAssignableFrom(type) && !type.IsAbstract)
        ];
        _examplesCount = _exampleTypes.Length;

        return true;
    }

    private int Loop()
    {
        var isExiting = false;
        var lastTime = 0.0f;

        while (!isExiting)
        {
            SDL_Event e;
            if (SDL_PollEvent(&e))
            {
                var eventType = (SDL_EventType)e.type;
                HandleEvent(eventType, ref isExiting, e);
            }

            var newTime = SDL_GetTicks() / 1000.0f;
            var deltaTime = newTime - lastTime;
            lastTime = newTime;

            if (!Frame(deltaTime))
            {
                return 1;
            }
        }

        return 0;
    }

    private void HandleEvent(
        SDL_EventType eventType,
        ref bool isExiting,
        SDL_Event e)
    {
        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_QUIT:
                _currentExample?.QuitInternal();
                _currentExample = null;
                isExiting = true;
                break;
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            {
                var key = e.key.scancode;
                if (key == SDL_Scancode.SDL_SCANCODE_2)
                {
                    _goToExampleIndex = _exampleIndex + 1;
                    if (_goToExampleIndex >= _examplesCount)
                    {
                        _goToExampleIndex = 0;
                    }
                }
                else if (key == SDL_Scancode.SDL_SCANCODE_1)
                {
                    _goToExampleIndex = _exampleIndex - 1;
                    if (_goToExampleIndex < 0)
                    {
                        _goToExampleIndex = _examplesCount - 1;
                    }
                }
                else
                {
                    _currentExample?.KeyboardEvent(e.key);
                }

                break;
            }
        }
    }

    private bool Frame(float deltaTime)
    {
        if (_goToExampleIndex != -1)
        {
            var previousExample = _currentExample;
            if (previousExample != null)
            {
                _currentExample = null;
                previousExample.QuitInternal();
                // ReSharper disable once RedundantAssignment
#pragma warning disable IDE0059
                previousExample = null;
#pragma warning restore IDE0059

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            _exampleIndex = _goToExampleIndex;
            _currentExample = (ExampleBase)Activator.CreateInstance(_exampleTypes[_exampleIndex])!;
            Console.WriteLine("STARTING EXAMPLE: " + _currentExample.Name);
            var isExampleInitialized = _currentExample.InitializeInternal();
            if (!isExampleInitialized)
            {
                Console.Error.WriteLine("Init failed!");
                return false;
            }

            _goToExampleIndex = -1;
        }

        if (_currentExample != null)
        {
            if (!_currentExample.Update(deltaTime))
            {
                Console.Error.WriteLine("Update failed!");
                return false;
            }

            if (!_currentExample.Draw(deltaTime))
            {
                Console.Error.WriteLine("Draw failed!");
                return false;
            }
        }

        return true;
    }
}
