using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.LowLevel;


//Originally written by Vengarioth
// https://www.github.com/Vengarioth

namespace Snowdrama
{
    public struct UnityPlayerLoopInserter
    {
        private PlayerLoopSystem _currentPlayerLoop;

        public static UnityPlayerLoopInserter GetCurrent()
        {
            var currentPlayerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            var inserter = new UnityPlayerLoopInserter();
            inserter._currentPlayerLoop = currentPlayerLoop;
            return inserter;
        }

        public void Insert(Type system, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            if (IsInsertedRecursive(system, _currentPlayerLoop))
            {
                return;
            }

            if (_currentPlayerLoop.subSystemList == null)
            {
                _currentPlayerLoop.subSystemList = new PlayerLoopSystem[1];
            }
            else
            {
                Array.Resize(ref _currentPlayerLoop.subSystemList, _currentPlayerLoop.subSystemList.Length + 1);
            }

            _currentPlayerLoop.subSystemList[_currentPlayerLoop.subSystemList.Length - 1] = new PlayerLoopSystem
            {
                loopConditionFunction = IntPtr.Zero,
                updateFunction = IntPtr.Zero,
                subSystemList = null,
                type = system,
                updateDelegate = updateDelegate,
            };
        }

        public bool InsertBefore(Type container, Type system, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            if (IsInsertedRecursive(system, _currentPlayerLoop))
            {
                return true;
            }

            return InsertBeforeRecursive(container, system, updateDelegate, ref _currentPlayerLoop);
        }

        private bool InsertBeforeRecursive(Type container, Type system, PlayerLoopSystem.UpdateFunction updateDelegate, ref PlayerLoopSystem currentSystem)
        {
            if (currentSystem.type == container)
            {
                if (currentSystem.subSystemList == null)
                {
                    currentSystem.subSystemList = new PlayerLoopSystem[1];
                }
                else
                {
                    var subSystemList = new PlayerLoopSystem[currentSystem.subSystemList.Length + 1];
                    Array.Copy(currentSystem.subSystemList, 0, subSystemList, 1, currentSystem.subSystemList.Length);
                    currentSystem.subSystemList = subSystemList;
                }

                currentSystem.subSystemList[0] = new PlayerLoopSystem
                {
                    loopConditionFunction = IntPtr.Zero,
                    updateFunction = IntPtr.Zero,
                    subSystemList = null,
                    type = system,
                    updateDelegate = updateDelegate,
                };

                return true;
            }
            else
            {
                if (currentSystem.subSystemList != null)
                {
                    for (int i = 0; i < currentSystem.subSystemList.Length; i++)
                    {
                        if (InsertBeforeRecursive(container, system, updateDelegate, ref currentSystem.subSystemList[i]))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool InsertAfter(Type container, Type system, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            if (IsInsertedRecursive(system, _currentPlayerLoop))
            {
                return true;
            }

            return InsertAfterRecursive(container, system, updateDelegate, ref _currentPlayerLoop);
        }

        private bool InsertAfterRecursive(Type container, Type system, PlayerLoopSystem.UpdateFunction updateDelegate, ref PlayerLoopSystem currentSystem)
        {
            if (currentSystem.type == container)
            {
                if (currentSystem.subSystemList == null)
                {
                    currentSystem.subSystemList = new PlayerLoopSystem[1];
                }
                else
                {
                    Array.Resize(ref currentSystem.subSystemList, currentSystem.subSystemList.Length + 1);
                }

                currentSystem.subSystemList[currentSystem.subSystemList.Length - 1] = new PlayerLoopSystem
                {
                    loopConditionFunction = IntPtr.Zero,
                    updateFunction = IntPtr.Zero,
                    subSystemList = null,
                    type = system,
                    updateDelegate = updateDelegate,
                };

                return true;
            }
            else
            {
                if (currentSystem.subSystemList != null)
                {
                    for (int i = 0; i < currentSystem.subSystemList.Length; i++)
                    {
                        if (InsertAfterRecursive(container, system, updateDelegate, ref currentSystem.subSystemList[i]))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsInsertedRecursive(Type type, PlayerLoopSystem currentSystem)
        {
            if (currentSystem.type == type)
            {
                return true;
            }

            if (currentSystem.subSystemList != null)
            {
                for (int i = 0; i < currentSystem.subSystemList.Length; i++)
                {
                    if (IsInsertedRecursive(type, currentSystem.subSystemList[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Flush()
        {
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(_currentPlayerLoop);
        }
    }
}