using System;
using System.Diagnostics;
using NUnit.Framework;
using gdio.unity_api;
using gdio.unity_api.v2;
using gdio.common.objects;
using System.Net.Http.Headers;
using System.Threading;
using NUnit.Framework.Legacy;



// using OpenQA.Selenium.Appium;
// using OpenQA.Selenium.Support.UI;
// using OpenQA.Selenium.Appium.iOS;

namespace Boss_Room_Tests
{
    /// <summary>
    /// TextFixtures for setting up various configurations of how GameDriver tests can be run
    /// Parameters:
    /// Target Mode [IDE|standalone]
    /// Host [localhost|IP Address|hostname]
    /// Platform [desktop|mobile|Xbox|PlayStation|Switch|XR]
    /// Path (optional) [string]
    /// </summary>
    ///
    
    
    [TestFixture("IDE", "localhost", "desktop", "offline", "//Player[@name='PlayerAvatar0']")]
    [TestFixture("standalone", "localhost", "desktop", "offline", "//Player[@name='PlayerAvatar0']", "/Users/Rob/Documents/Unity_Builds/boss_room.app")]
    [TestFixture("standalone", "192.168.0.152", "ios", "offline", "//Player[@name='PlayerAvatar0']", "com.unity.boss_room.ipa")]
    [TestFixture("standalone", "192.168.0.153", "android", "offline", "//Player[@name='PlayerAvatar0']", "com.unity.boss_room.apk")]
    [TestFixture("standalone", "192.168.0.154", "xbox", "offline", "//Player[@name='PlayerAvatar0']", "com.unity.boss_room.xbe")]
    
    public class UnitTest
    {
        private static string _testMode, _host, _platform, _gameMode, _player, _path;
        private ApiClient _api;

        public UnitTest(string testMode, string host, string platform, string gameMode, string player)
        {
            _testMode = testMode;
            _host = host;
            _platform = platform;
            _gameMode = gameMode;
            _player = player;
            _path = null;
        }

        public UnitTest(string testMode, string host, string platform, string gameMode, string player, string path)
        {
            _testMode = testMode;
            _host = host;
            _platform = platform;
            _gameMode = gameMode;
            _player = player;
            _path = path;
        }
        
        //These parameters can be used to override settings used to test when running from the NUnit command line
        // public string testHost = TestContext.Parameters.Get("Host", host);
        // public string testMode = TestContext.Parameters.Get("Mode", target);
        // public string pathToExe = TestContext.Parameters.Get("pathToExe", exePath);
        public string lobbyMode = TestContext.Parameters.Get("lobby", _gameMode);
        
        private string _currentAnim;

        [OneTimeSetUp]
        public void Connect()
        {
            try
            {
                // Workaround: MacOS blocks broadcast addresses
                ApiClient.AUTOPLAY_BROADCAST_ADDR = "127.0.0.1";
                
                // Instantiate the ApiClient
                _api = new ApiClient();
                // If an executable path was supplied, we will launch the standalone game
                if (_path != null) 
                    ApiClient.Launch(_path);

                _api.Connect(_host);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            _api.UnityLoggedMessage += (s, e) =>
            {
                Console.WriteLine($"Type: {e.type.ToString()}\r\nCondition: {e.condition}\r\nStackTrace: {e.stackTrace}");
            };

            _api.EnableHooks(HookingObject.KEYBOARD);
            _api.EnableHooks(HookingObject.MOUSE);

            if (_gameMode == "online")
            {
                StartOnlineLobby();
            }
            else StartSinglePlayerLobby();
            
            _api.Wait(1000);
        }

        [OneTimeTearDown]
        public void Disconnect()
        {
            // Disconnect the GameDriver client from the agent
            _api.Wait(3000);
            _api.DisableHooks(HookingObject.ALL);
            _api.Wait(2000);
            if (_testMode == "IDE")
            {
                _api.ToggleEditorPlay();
            }
            _api.Disconnect();
            _api.Wait(2000);
            
            Console.WriteLine("Total tests completed: {0}", TestContext.CurrentContext.Result.PassCount + TestContext.CurrentContext.Result.FailCount);
            Console.WriteLine("Total tests passed: {0}", TestContext.CurrentContext.Result.PassCount);
            Console.WriteLine("Total tests failed: {0}", TestContext.CurrentContext.Result.FailCount);
        }


        [Test, Order(1)]
        [TestCase(0, "TANK")]
        [TestCase(1, "TANK")]
        [TestCase(2, "ARCHER")]
        [TestCase(3, "ARCHER")]
        [TestCase(4, "MAGE")]
        [TestCase(5, "MAGE")]
        [TestCase(6, "ROGUE")]
        [TestCase(7, "ROGUE")]
        public void SmokeTest_ClassAbilities(int classId, string className)
        {
            Assert.Multiple(() =>
            {
                ClassicAssert.AreEqual(SelectPlayer(classId), className);
                
                //SelectPlayer(classId);
                
                // Clear Explainer
                DismissHowToPlayDialog();
                
                // Test the character's abilities
                TestAbility(KeyCode.Alpha1, 1);
                TestAbility(KeyCode.Alpha2, 2);
                TestAbility(KeyCode.Alpha3, 3);
                _api.Click(MouseButtons.LEFT, 10);
                
                // Test the character's emotes
                TestAbility(KeyCode.Alpha4, 4);
                TestAbility(KeyCode.Alpha5, 5); 
                TestAbility(KeyCode.Alpha6, 6);
                TestAbility(KeyCode.Alpha7, 7);
                TestAbility(KeyCode.Alpha8, 8);
                
                // Kill Self and Reset
                KillSelf();

                if (_api.GetSceneName() == "PostGame")
                {
                    _api.Wait(2000);
                    _api.WaitForObject("//*[@name='PlayAgainBtn'");
                    _api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                    _api.Wait(3000);
                }
                else
                {
                    _api.LoadScene("CharSelect");
                }

                //ResetGame();
                
                //_api.Wait(3000);
                //_api.LoadScene("PostGame");
            });
        }
        
        //[Test, Order(8)]
        /// <summary>
        /// Below is the original, raw format test
        /// </summary>
        public void T08_Male_Rogue_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                _api.MouseMoveToObject(" //*[@name='PlayerSeat (7)']", 30);
                _api.Wait(3000);

                
                //Select character using mouse left-click
                _api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (7)']", 30);
                //_api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (7)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                _api.WaitForEmptyInput();

                //Click READY button
                _api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                _api.Wait(7000);

                //Close cheats panel
                _api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                _api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                _api.Wait(3000);

                //Close How To Play Panel
                _api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                _api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                _api.Wait(3000);

                //Press the "1" key to activate the first ability
                _api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                _api.Wait(100);

                //Get the current animation and validate it is correct
                _currentAnim = _api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.That(_currentAnim.Equals("Attacks.Attack1"), "Incorrect Animation Playing");
                _api.Wait(3000);

                //Press the "2" key to activate the second ability
                _api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 120);
                _api.Wait(400);

                //Get the current animation and validate it is correct
                _currentAnim = _api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.That(_currentAnim.Equals("Attacks.Dash Attack (start)"), "Incorrect Animation Playing");
                _api.Wait(3000);

                //Press the "3" key to activate the third ability
                _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
                _api.Wait(700);

                //Get the current animation and validate it is correct
                _currentAnim = _api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.That(_currentAnim.Equals("Attacks.Buff1"), "Incorrect Animation Playing");
                _api.Wait(5000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                _api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1);
                _api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0);
                _api.Wait(5000);

                //Click "Play Again"
                _api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                _api.Wait(5000);

            });
        }

        [Test, Order(2)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public void SmokeTest_TakeDamage(int classId)
        {
            Assert.Multiple(() =>
            {
                SelectPlayer(classId);
                
                // Clear Explainer
                DismissHowToPlayDialog();
                
                // Move Character
                int startingHp = _api.GetObjectFieldValue<int>("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value");
            
                //Kill all Imps
                // _api.ClickObject(MouseButtons.LEFT, "//*[@name='KillAllEnemiesButton']", 30);
                // _api.Wait(1000);
            
                //Spawn one Imp
                // _api.ClickObject(MouseButtons.LEFT, "//*[@name='SpawnImpButton']", 30);
                // _api.Wait(1000);
            
                //Move the player to the Imp's position
                _api.SetObjectFieldValue("//Player[@name='PlayerAvatar0']/fn:component('UnityEngine.Transform')", "position", _api.GetObjectPosition("//*[@name='Imp(Clone)']"));
                _api.Wait(5000);
                
                int endingHp = _api.GetObjectFieldValue<int>("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value");

                
                Assert.That(endingHp < startingHp, String.Format("Starting HP = {0}, Ending HP = {1}", startingHp, endingHp));
                
                // Kill Self and Reset
                KillSelf();

                if (_api.GetSceneName() == "PostGame")
                {
                    _api.Wait(2000);
                    _api.WaitForObject("//*[@name='PlayAgainBtn'");
                    _api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                    _api.Wait(3000);
                }
                else
                {
                    _api.LoadScene("CharSelect");
                }

                //ResetGame();
                
                //_api.Wait(3000);
                //_api.LoadScene("PostGame");
            });
        }
        /// <summary>
        /// Below is the original, raw format Receive Damage test
        /// </summary>
        //[Test, Order(9)]
        public void T09_Test_Recieve_Damage()
        {
            //Select First Player
            _api.MouseMoveToObject(" //*[@name='PlayerSeat (0)']", 30);
            _api.Wait(3000);
            _api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (0)']", 30);
            _api.Wait(3000);

            //Click "Ready" button
            _api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
            _api.Wait(7000);
            
            //check the starting HP
            int startingHp = _api.GetObjectFieldValue<int>("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value");
            
            //Kill all Imps
            _api.ClickObject(MouseButtons.LEFT, "//*[@name='KillAllEnemiesButton']", 30);
            _api.Wait(1000);
            
            //Spawn one Imp
            _api.ClickObject(MouseButtons.LEFT, "//*[@name='SpawnImpButton']", 30);
            _api.Wait(1000);
            
            //Move the player to the Imp's position
            _api.SetObjectFieldValue("//Player[@name='PlayerAvatar0']/fn:component('UnityEngine.Transform')", "position", _api.GetObjectPosition("//*[@name='Imp(Clone)']"));

            //Close cheats panel
            _api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
            _api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
            _api.Wait(3000);

            //Close How To Play Panel
            _api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
            _api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
            _api.Wait(3000);

            //Validate player HP did change
            int endHp = _api.GetObjectFieldValue<int>("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value");
            Assert.That(endHp < startingHp, "Player did not Recieve Damage!");

            KillSelf();
        }

        //[Test, Order(10)]
        public void T10_KillBoss()
        {
            Assert.Multiple(() =>
            {
                bool bossLife = false;
                int numHits = 0;

                //Hover mouse over character selection based on seat position
                _api.MouseMoveToObject(" //*[@name='PlayerSeat (2)']", 30);
                _api.Wait(3000);

                //Select character using mouse left-click
                _api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (2)']", 30);
                //_api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (2)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                _api.WaitForEmptyInput();

                //Click READY button
                _api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                _api.Wait(4000);

                //Close cheats panel
                //_api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                //_api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                //_api.Wait(3000);

                //Close How To Play Panel
                _api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']");
                _api.MouseMoveToObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                _api.Wait(1000);
                _api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                _api.Wait(3000);


                //Enable God Mode Cheat
                //_api.CallMethod("//*[@name='DebugCheatsManager']/fn:component('Unity.BossRoom.DebugCheats.DebugCheatsManager')", "ToggleGodMode");

                //Kill All Enemies
                KillEnemies();

                //Move towards the door
                _api.CallMethod("/*[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacterMovement')", "SetMovementTarget", new object[] { new Vector3(112, 0, 35) });
                _api.Wait(1000);
                
                //Open the door
                _api.SetObjectFieldValue("//*[@name='InteractiveBossDoor']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.SwitchedDoor')", "ForceOpen", true);
                _api.Wait(10000);

                //Start a SmartAgent to monitor boss health and signal to Attack
                string hpListener = _api.ScheduleScript(@"local hitPoints = ResolveObject(""//*[@name='ImpBoss(Clone)']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value"");
                    if hitPoints > 0 then
                        Notify(true)
                    end", ScriptExecutionMode.EveryNthFrames, (int)_api.GetLastFPS() * 3);

                _api.ScriptSignal += (sender, args) => {
                    bossLife = (bool)args.obj;
                    //Console.WriteLine($"Boss life = {hitPoints}");
                    HandleAttackBoss();
                };

                _api.Wait(6000);

                while (_api.GetObjectFieldValue<int>("//*[@name='ImpBoss(Clone)']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value") > 0)
                //while (hitPoints == true)
                {
                    numHits++;
                    Console.WriteLine($"Boss life = {bossLife}. Continuing");
                    Thread.Sleep(3000);
                }

                Console.WriteLine($"Number of attacks = {numHits}");

                //Stop the SmartAgent
                _api.UnscheduleScript(hpListener);

                //Kill player by setting hp to 0
                //KillSelf();

                //Wait for Loading screen to open and close
                //_api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                //_api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                _api.Wait(15000);

                //Click "Play Again"
                _api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                _api.Wait(5000);

            });
        }

        [Test, Order(99)]
        public void ScreenshotTest()
        {
            _api.Wait(2000);
            _api.CaptureScreenshot("Test.jpg", false, true);
            _api.Wait(2000);
        }
        
        private string SelectPlayer(int playerId)
        {
            return SelectPlayer(playerId.ToString());
        }
        
        private string SelectPlayer(string player)
        {
            var playerClassName = "Unknown";

            try
            {
                // Hover mouse over character selection based on seat position
                _api.MouseMoveToObject(String.Format("//*[@name='PlayerSeat ({0})']", player), 30);
                _api.Wait(500);

                // Select the character using mouse left-click
                _api.ClickObject(MouseButtons.LEFT, String.Format("//*[@name='PlayerSeat ({0})']/*[@name='AnimationContainer']/*[@name='ClickInteract']", player), 30);
                _api.WaitForEmptyInput();
                
                // Get the character class name
                playerClassName =
                    _api.GetObjectFieldValue<string>(
                        "//*[@name='CurrentClass (TMP)']/fn:component('TMPro.TextMeshProUGUI')/@text");

            }
            catch (Exception e)
            {
                Assert.Fail("Unable to select player" + playerClassName);
            }

            try
            {
                //Click READY button
                _api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                _api.Wait(6000);
            }
            catch
            {
                Assert.Fail("Failed to select ready button");
            }

            return playerClassName;
        }

        private void DismissHowToPlayDialog()
        {
            //Close How To Play Panel
            _api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
            _api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
            _api.Wait(1000);
        }

        private void TestAbility(KeyCode key, int keyInt)
        {
            _api.KeyPress(new KeyCode[] { key }, 30);
            _api.Wait(2500);

            //Get the current animation and validate it is correct
            //_currentAnim = _api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
            
            // TODO: Animators are called Attack and Attack2, not Attack1
            //Assert.That(String.Format("Attacks.Attack", keyInt) == _currentAnim, "Incorrect Animation Playing");
            //_api.Wait(1000);
        }

        private void ResetGame()
        {
            // Wait for Game to Reset
            _api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1);
            _api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0);
            _api.Wait(3000);
            
            //Click "Play Again"
            _api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
            _api.Wait(3000);
        }

        private void KillSelf()
        {
            _api.Wait(2000);
            _api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new gdio.common.lookup.HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -10000 });
            _api.Wait(3000);
        }

        private void StartOnlineLobby()
        {
            _api.Wait(5000);
            _api.CallMethod("//*[@name='Lobby Start Button']/fn:component('UnityEngine.UI.Button')", "Press");
            _api.Wait(5000);
            _api.CallMethod("/*[@name='UI Canvas']/*[@name='LobbyPopup']/*[@name='Tab Buttons']/*[@name='CreateButton']/fn:component('UnityEngine.UI.Button')", "Press");
            _api.Wait(5000);
            _api.CallMethod("/*[@name='UI Canvas']/*[@name='LobbyPopup']/*[@name='Tabs']/*[@name='LobbyCreationUI']/*[@name='Lobby Name Input Field']/*[@name='InputText']/fn:component('UnityEngine.UI.Text')", "set_text", new object[] { "GameDriverLobby" });
            _api.Wait(5000);
            _api.CallMethod("/*[@name='UI Canvas']/*[@name='LobbyPopup']/*[@name='Tabs']/*[@name='LobbyCreationUI']/*[@name='Create Lobby Button']/fn:component('UnityEngine.UI.Button')", "Press");
            _api.Wait(5000);
        }

        public void StartSinglePlayerLobby()
        {
            _api.ClickObject(MouseButtons.LEFT, "//*[@name='IP Start Button']", 30);
            _api.Wait(2000);
            _api.ClickObject(MouseButtons.LEFT, "//*[@name='Host IP Connection Button']", 30);
            _api.Wait(4000);
            //_api.ClickObject(MouseButtons.LEFT, "/*[@name='NetworkSimulator']/*[@name='NetworkSimulatorUICanvas']/*[@name='NetworkSimulatorPopupPanel']/*[@name='Cancel Button']", 30);
            //_api.Wait(3000);
        }

        public void HandleInput(string platform, string path) 
        {
            if (platform == "mobile")
            {
                _api.TapObject(path, 1, 30);
            }
            else if (platform == "desktop") 
            {
                _api.ClickObject(MouseButtons.LEFT, path, 30);
            }
        }

        public void BossFight() 
        {
            string hpListener = _api.ScheduleScript(@"local hitPoints = ResolveObject(""//*[@name='ImpBoss(Clone)']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value"");
                if hitPoints > 0 then
                    Notify(true)
                end", ScriptExecutionMode.EveryNthFrames, (int)_api.GetLastFPS() * 3);

            _api.ScriptSignal += (sender, args) => {
                Console.WriteLine("Boss Still alive! Attacking Boss!");
                //HandleAttackBoss();
            };
        }

        public void HandleAttackBoss() 
        {
            Console.WriteLine("Boss Still alive! Attacking Boss!");
            _api.ClickObject(MouseButtons.LEFT, "//*[@name='ImpBoss(Clone)']", 5);

            //Use Ability 3
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 1);
            _api.Wait(300);
            _api.Click(MouseButtons.LEFT, 1);
            _api.Wait(500);

        }

        public void KillEnemies()
        {
            string portalOne = "/*[@name='Entrance']/*[@name='spawn_door (4)']/*[@name='door_crystral_base']";
            string portalTwo = "/*[@name='Entrance']/*[@name='spawn_door (4)']/*[@name='door_crystral_base (1)']";

            //Turn on God Mode
            /*
            _api.KeyPress(new KeyCode[] { KeyCode.Slash }, 30);
            _api.Wait(1000);
            //_api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='ToggleGodModeButton']", 30);
            _api.Wait(2000);
            _api.KeyPress(new KeyCode[] { KeyCode.Slash }, 30);
            _api.Wait(5000);
            */
            _api.CallMethod("//*[@name='DebugCheatsManager']/fn:component('Unity.BossRoom.DebugCheats.DebugCheatsManager')", "ToggleGodMode");

            //Kill the portal that spawns more enemies
            _api.CallMethod("/*[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacterMovement')", "SetMovementTarget", new object[] { _api.GetObjectPosition(portalOne) });
            _api.Wait(15000);
            _api.MouseMoveToObject(portalOne, 30);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(1500);
            _api.Click(MouseButtons.LEFT, 30);
            _api.Wait(1500);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(1500);
            _api.Click(MouseButtons.LEFT, 30);
            _api.Wait(1500);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(1500);
            _api.Click(MouseButtons.LEFT, 30);
            _api.Wait(1500);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(1500);
            _api.Click(MouseButtons.LEFT, 30);
            _api.Wait(1500);

            _api.MouseMoveToObject(portalTwo, 30);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(1500);
            _api.Click(MouseButtons.LEFT, 30);
            _api.Wait(1500);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(500);
            _api.Click(MouseButtons.LEFT, 30);
            _api.Wait(1500);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(1500);
            _api.Click(MouseButtons.LEFT, 30);

            //kill the goblins
            _api.Wait(1500);
            _api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            _api.Wait(1500);
            _api.Click(MouseButtons.LEFT, 30);
            _api.Wait(1500);

        }
    }
}