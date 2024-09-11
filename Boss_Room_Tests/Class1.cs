using System;
using System.Diagnostics;
using NUnit.Framework;
using gdio.unity_api;
using gdio.unity_api.v2;
using gdio.common.objects;
using System.Net.Http.Headers;
using System.Threading;

namespace DemoTest
{
    [TestFixture]
    public class UnitTest
    {

        private static string target = "IDE";
        //private string target = "standalone";

        private static string platform = "desktop";
        //private statuc string platform = "mobile";

        private static string gameMode = "offline";
        //public string gameMode = "online";

        private static string exePath = null;
        //private static string exePath = "c:\Path\To\Game.exe";

        //These parameters can be used to override settings used to test when running from the NUnit command line
        public string testMode = TestContext.Parameters.Get("Mode", target);
        public string pathToExe = TestContext.Parameters.Get("pathToExe", exePath);
        public string lobbyMode = TestContext.Parameters.Get("lobby", gameMode);

        string player = "//Player[@name='PlayerAvatar0']";
        string currentAnim;

        ApiClient api;

        [OneTimeSetUp]
        public void Connect()
        {
            try
            {
                // First we need to create an instance of the ApiClient
                api = new ApiClient();

                // If an executable path was supplied, we will launch the standalone game
                if (pathToExe != null)
                {
                    ApiClient.Launch(pathToExe);
                    api.Connect("localhost", 19734, false, 30);
                }

                // If no executable path was given, we will attempt to connect to the Unity editor and initiate Play mode
                else if (testMode == "IDE")
                {
                    api.Connect("localhost", 19734, true, 30);
                }
                // Otherwise, attempt to connect to an already playing game
                else api.Connect("localhost");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            api.UnityLoggedMessage += (s, e) =>
            {
                Console.WriteLine($"Type: {e.type.ToString()}\r\nCondition: {e.condition}\r\nStackTrace: {e.stackTrace}");
            };

            api.EnableHooks(HookingObject.ALL);

            if (gameMode == "online")
            {
                StartOnlineLobby();
            }
            else StartSinglePlayerLobby();
            
            api.Wait(3000);
        }
        

        [Test, Order(1)]
        public void T01_Female_Tank_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject("//*[@name='PlayerSeat (0)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayerSeat (0)']/*[@name='AnimationContainer']/*[@name='ClickInteract']", 30);
                //api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                //api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='Hero Action Bar']/*[@name='Button0']", 300);
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(500);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreNotEqual("Attacks.Attack1", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 30);
                api.Wait(1000);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                api.Wait(1000);
                Assert.AreEqual("Attacks.TankShieldBuff (start)", currentAnim, "Incorrect Animation Playing");
                api.Wait(12000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0,true, 60);
                api.Wait(5000);
            
                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });
        }
        
        
        [Test, Order(2)]
        public void T02_Male_Tank_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (1)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (1)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (1)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(100);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                //Assert.AreEqual("Attacks.Attack1", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 30);
                api.Wait(100);

                ////Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                //Assert.AreEqual("Attacks.TankShieldBuff (start)", currentAnim, "Incorrect Animation Playing");
                api.Wait(10000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });

        }


        [Test, Order(3)]
        public void T03_Female_Ranger_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (2)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (2)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (2)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(100);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                //Assert.AreEqual("Attacks.Attack1", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 120);
                api.Wait(400);
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                //Assert.AreEqual("Attacks.Archer Charged Shot (start)", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);
            
                //Press the "3" key then click to activate the third ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
                api.Wait(500);
                api.ClickObject(MouseButtons.LEFT, "//*[@name='BreakablePot (1)']", 30);
                api.Wait(150);
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
            
                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                api.Wait(5000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });

        }
        [Test, Order(4)]
        public void T04_Male_Ranger_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (3)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (3)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (3)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(100);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Attack1", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 120);
                api.Wait(400);
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Archer Charged Shot (start)", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "3" key then click to activate the third ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
                api.Wait(500);
                api.ClickObject(MouseButtons.LEFT, "//*[@name='BreakablePot (1)']", 30);
                api.Wait(150);
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });

                ////Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                api.Wait(5000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });

        }

        [Test, Order(5)]
        public void T05_Female_Healer_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (4)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (4)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (4)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Click The target to attack
                api.ClickObject(MouseButtons.LEFT, "//*[@name='BreakablePot (1)']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(100);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Attack1", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 120);
                api.Wait(400);
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.SkillHeal", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });

        }

        [Test, Order(6)]
        public void T06_Male_Healer_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (5)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (5)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (4)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Click The target to attack
                api.ClickObject(MouseButtons.LEFT, "//*[@name='BreakablePot (1)']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(100);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Attack1", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 120);
                api.Wait(400);
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.SkillHeal", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });


        }

        [Test, Order(7)]
        public void T07_Female_Rogue_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (6)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (6)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (6)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']]", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(100);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Attack1", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 120);
                api.Wait(400);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Dash Attack (start)", currentAnim, "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "3" key to activate the third ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
                api.Wait(500);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Buff1", currentAnim, "Incorrect Animation Playing");
                api.Wait(5000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });

        }

        [Test, Order(8)]
        public void T08_Male_Rogue_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (7)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (7)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (7)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Press the "1" key to activate the first ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha1 }, 30);
                api.Wait(100);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.That(currentAnim.Equals("Attacks.Attack1"), "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "2" key to activate the second ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 120);
                api.Wait(400);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.That(currentAnim.Equals("Attacks.Dash Attack (start)"), "Incorrect Animation Playing");
                api.Wait(3000);

                //Press the "3" key to activate the third ability
                api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
                api.Wait(700);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.That(currentAnim.Equals("Attacks.Buff1"), "Incorrect Animation Playing");
                api.Wait(5000);

                //Kill player by setting hp to 0
                KillSelf();

                //Wait for Loading screen to open and close
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });

        }

        [Test, Order(9)]
        public void T09_Test_Recieve_Damage()
        {
            //Select First Player
            api.MouseMoveToObject(" //*[@name='PlayerSeat (0)']", 30);
            api.Wait(3000);
            api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (0)']", 30);
            api.Wait(3000);

            //Click "Ready" button
            api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
            api.Wait(7000);
            
            //check the starting HP
            int startingHp = api.GetObjectFieldValue<int>("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value");
            
            //Kill all Imps
            api.ClickObject(MouseButtons.LEFT, "//*[@name='KillAllEnemiesButton']", 30);
            api.Wait(1000);
            
            //Spawn one Imp
            api.ClickObject(MouseButtons.LEFT, "//*[@name='SpawnImpButton']", 30);
            api.Wait(1000);
            
            //Move the player to the Imp's position
            api.SetObjectFieldValue("//Player[@name='PlayerAvatar0']/fn:component('UnityEngine.Transform')", "position", api.GetObjectPosition("//*[@name='Imp(Clone)']"));

            //Close cheats panel
            api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
            api.Wait(3000);

            //Close How To Play Panel
            api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
            api.Wait(3000);

            //Validate player HP did change
            int endHp = api.GetObjectFieldValue<int>("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value");
            Assert.Less(endHp, startingHp, "Player did not Recieve Damage!");

            KillSelf();
        }

        [Test, Order(10)]
        public void T10_KillBoss()
        {
            Assert.Multiple(() =>
            {
                bool bossLife = false;
                int numHits = 0;

                //Hover mouse over character selection based on seat position
                api.MouseMoveToObject(" //*[@name='PlayerSeat (2)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (2)']", 30);
                //api.WaitForObjectValue("/*[@name='CharacterSelectCanvas']/*[@name='PlayerSeats']/*[@name='PlayerSeat (2)']/*[@name='AnimationContainer']/*[@name='ActiveBkgnd']/*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(4000);

                //Close cheats panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
                api.WaitForObject("/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']");
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
                api.Wait(3000);

                //Kill All Enemies
                KillEnemies();

                //Move towards the door
                api.CallMethod("/*[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacterMovement')", "SetMovementTarget", new object[] { new Vector3(112, 0, 35) });
                api.Wait(1000);
                
                //Open the door
                api.SetObjectFieldValue("//*[@name='InteractiveBossDoor']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.SwitchedDoor')", "ForceOpen", true);
                api.Wait(10000);

                //Start a SmartAgent to monitor boss health and signal to Attack
                string hpListener = api.ScheduleScript(@"local hitPoints = ResolveObject(""//*[@name='ImpBoss(Clone)']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value"");
                    if hitPoints > 0 then
                        Notify(true)
                    end", ScriptExecutionMode.EveryNthFrames, (int)api.GetLastFPS() * 3);

                api.ScriptSignal += (sender, args) => {
                    bossLife = (bool)args.obj;
                    //Console.WriteLine($"Boss life = {hitPoints}");
                    HandleAttackBoss();
                };

                api.Wait(6000);

                while (api.GetObjectFieldValue<int>("//*[@name='ImpBoss(Clone)']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value") > 0)
                //while (hitPoints == true)
                {
                    numHits++;
                    Console.WriteLine($"Boss life = {bossLife}. Continuing");
                    Thread.Sleep(3000);
                }

                Console.WriteLine($"Number of attacks = {numHits}");

                //Stop the SmartAgent
                api.UnscheduleScript(hpListener);

                //Kill player by setting hp to 0
                //KillSelf();

                //Wait for Loading screen to open and close
                //api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 1, true, 60);
                //api.WaitForObjectValue("//*[@name='LoadingScreen']/fn:component('UnityEngine.CanvasGroup')", "@alpha", 0, true, 60);
                api.Wait(5000);

                //Click "Play Again"
                api.ClickObject(MouseButtons.LEFT, "//*[@name='PlayAgainBtn']", 30);
                api.Wait(5000);

            });
        }
        
        [OneTimeTearDown]
        public void Disconnect()
        {
            // Disconnect the GameDriver client from the agent
            api.Wait(3000);
            api.DisableHooks(HookingObject.ALL);
            api.Wait(2000);
            api.Disconnect();
            api.Wait(2000);
        }

        public void KillSelf()
        {
            api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });
        }

        public void StartOnlineLobby()
        {
            api.Wait(5000);
            api.CallMethod("//*[@name='Lobby Start Button']/fn:component('UnityEngine.UI.Button')", "Press", null);
            api.Wait(5000);
            api.CallMethod("/*[@name='UI Canvas']/*[@name='LobbyPopup']/*[@name='Tab Buttons']/*[@name='CreateButton']/fn:component('UnityEngine.UI.Button')", "Press", null);
            api.Wait(5000);
            api.CallMethod("/*[@name='UI Canvas']/*[@name='LobbyPopup']/*[@name='Tabs']/*[@name='LobbyCreationUI']/*[@name='Lobby Name Input Field']/*[@name='InputText']/fn:component('UnityEngine.UI.Text')", "set_text", new object[] { "GameDriverLobby" });
            api.Wait(5000);
            api.CallMethod("/*[@name='UI Canvas']/*[@name='LobbyPopup']/*[@name='Tabs']/*[@name='LobbyCreationUI']/*[@name='Create Lobby Button']/fn:component('UnityEngine.UI.Button')", "Press", null);
            api.Wait(5000);
        }

        public void StartSinglePlayerLobby()
        {
            api.ClickObject(MouseButtons.LEFT, "//*[@name='IP Start Button']", 30);
            api.Wait(2000);
            api.ClickObject(MouseButtons.LEFT, "//*[@name='Host IP Connection Button']", 30);
            api.Wait(4000);
            //api.ClickObject(MouseButtons.LEFT, "/*[@name='NetworkSimulator']/*[@name='NetworkSimulatorUICanvas']/*[@name='NetworkSimulatorPopupPanel']/*[@name='Cancel Button']", 30);
            //api.Wait(3000);
        }

        public void HandleInput(string platform, string path) 
        {
            if (platform == "mobile")
            {
                api.TapObject(path, 1, 30);
            }
            else if (platform == "desktop") 
            {
                api.ClickObject(MouseButtons.LEFT, path, 30);
            }
        }

        public void BossFight() 
        {
            string hpListener = api.ScheduleScript(@"local hitPoints = ResolveObject(""//*[@name='ImpBoss(Clone)']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value"");
                if hitPoints > 0 then
                    Notify(true)
                end", ScriptExecutionMode.EveryNthFrames, (int)api.GetLastFPS() * 3);

            api.ScriptSignal += (sender, args) => {
                Console.WriteLine("Boss Still alive! Attacking Boss!");
                //HandleAttackBoss();
            };
        }

        public void HandleAttackBoss() 
        {
            Console.WriteLine("Boss Still alive! Attacking Boss!");
            api.ClickObject(MouseButtons.LEFT, "//*[@name='ImpBoss(Clone)']", 5);

            //Use Ability 3
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 1);
            api.Wait(300);
            api.Click(MouseButtons.LEFT, 1);
            api.Wait(500);

        }

        public void KillEnemies()
        {
            string portalOne = "/*[@name='Entrance']/*[@name='spawn_door (4)']/*[@name='door_crystral_base']";
            string portalTwo = "/*[@name='Entrance']/*[@name='spawn_door (4)']/*[@name='door_crystral_base (1)']";
            //Turn on God Mode

            api.KeyPress(new KeyCode[] { KeyCode.Slash }, 30);
            api.Wait(1000);
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='ToggleGodModeButton']", 30);
            api.Wait(2000);
            api.KeyPress(new KeyCode[] { KeyCode.Slash }, 30);
            api.Wait(5000);

            //Kill the portal that spawns more enemies
            api.CallMethod("/*[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacterMovement')", "SetMovementTarget", new object[] { api.GetObjectPosition(portalOne) });
            api.Wait(15000);
            api.MouseMoveToObject(portalOne, 30, true);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(1500);
            api.Click(MouseButtons.LEFT, 30);
            api.Wait(1500);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(1500);
            api.Click(MouseButtons.LEFT, 30);
            api.Wait(1500);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(1500);
            api.Click(MouseButtons.LEFT, 30);
            api.Wait(1500);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(1500);
            api.Click(MouseButtons.LEFT, 30);
            api.Wait(1500);

            api.MouseMoveToObject(portalTwo, 30, true);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(1500);
            api.Click(MouseButtons.LEFT, 30);
            api.Wait(1500);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(500);
            api.Click(MouseButtons.LEFT, 30);
            api.Wait(1500);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(1500);
            api.Click(MouseButtons.LEFT, 30);

            //kill the goblins
            api.Wait(1500);
            api.KeyPress(new KeyCode[] { KeyCode.Alpha3 }, 30);
            api.Wait(1500);
            api.Click(MouseButtons.LEFT, 30);
            api.Wait(1500);

        }
    }
}