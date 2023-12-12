using System;
using System.Diagnostics;
using NUnit.Framework;
using gdio.unity_api;
using gdio.unity_api.v2;
using gdio.common.objects;

namespace DemoTest
{
    [TestFixture]
    public class UnitTest
    {
        //These parameters can be used to override settings used to test when running from the NUnit command line
        public string testMode = TestContext.Parameters.Get("Mode", "IDE");
        public string pathToExe = TestContext.Parameters.Get("pathToExe", null); // replace null with the path to your executable as needed

       

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
                else api.Connect("localhost", 19734, false, 30);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // Enable input hooking
            api.EnableHooks(HookingObject.ALL);

            //Start the Game - in this example we're waiting for an object called "StartButton" to become active, then clicking it.
       
            api.Wait(3000);
        }

        [Test, Order(0)]
        public void StartSinglePlayerLobby()
        {
            
            api.ClickObject(MouseButtons.LEFT, "//*[@name='IP Start Button']", 30);
            api.Wait(2000);
            api.ClickObject(MouseButtons.LEFT, "//*[@name='Host IP Connection Button']", 30);
            api.Wait(4000);
            api.ClickObject(MouseButtons.LEFT, "/*[@name='NetworkSimulator']/*[@name='NetworkSimulatorUICanvas']/*[@name='NetworkSimulatorPopupPanel']/*[@name='Cancel Button']", 30);
            api.Wait(3000);
          
        }
        

        [Test, Order(1)]
        public void T01_Female_Tank_Class_Smoke_Tests()
        {
            Assert.Multiple(() =>
            {
            //Hover mouse over character selection based on seat position
            api.MouseMoveToObject(" //*[@name='PlayerSeat (0)']", 30);
            api.Wait(3000);
            
            //Select character using mouse left-click
            api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (0)']", 30);
            api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
            api.WaitForEmptyInput();
            
            //Click READY button
            api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
            api.Wait(7000);
            
            //Close cheats panel
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
            api.Wait(3000);
            
            //Close How To Play Panel
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
            api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 30);
            api.Wait(100);

            //Get the current animation and validate it is correct
            currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
            Assert.AreEqual("Attacks.TankShieldBuff (start)", currentAnim, "Incorrect Animation Playing");
            api.Wait(10000);

            //Kill player by setting hp to 0
            api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });
            
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
            api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
            api.WaitForEmptyInput();

            //Click READY button
            api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
            api.Wait(7000);

            //Close cheats panel
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
            api.Wait(3000);

            //Close How To Play Panel
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
            api.KeyPress(new KeyCode[] { KeyCode.Alpha2 }, 30);
            api.Wait(100);

            ////Get the current animation and validate it is correct
            currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
            Assert.AreEqual("Attacks.TankShieldBuff (start)", currentAnim, "Incorrect Animation Playing");
            api.Wait(10000);

            //Kill player by setting hp to 0
            api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });

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
            api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
            api.WaitForEmptyInput();

            //Click READY button
            api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
            api.Wait(7000);

            //Close cheats panel
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
            api.Wait(3000);

            //Close How To Play Panel
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
            api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });

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
                api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
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
                api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });

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
                api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
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
                api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });

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
                api.MouseMoveToObject(" //*[@name='PlayerSeat (4)']", 30);
                api.Wait(3000);

                //Select character using mouse left-click
                api.ClickObject(MouseButtons.LEFT, " //*[@name='PlayerSeat (4)']", 30);
                api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
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
                api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });

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
                api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
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
                api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });

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
                api.WaitForObjectValue("//*[@name='ActiveBkgnd']", "@activeInHierarchy", true);
                api.WaitForEmptyInput();

                //Click READY button
                api.ClickObject(MouseButtons.LEFT, "//*[@name='Ready Btn']", 30);
                api.Wait(7000);

                //Close cheats panel
                api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
                api.Wait(3000);

                //Close How To Play Panel
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
                api.Wait(700);

                //Get the current animation and validate it is correct
                currentAnim = api.CallMethod<String>("//*[@name='AvatarGraphics0']/fn:component('UnityEngine.Animator')", "GetAnimatorStateName", new object[] { 1, true });
                Assert.AreEqual("Attacks.Buff1", currentAnim, "Incorrect Animation Playing");
                api.Wait(5000);

                //Kill player by setting hp to 0
                api.CallMethod("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')", "ReceiveHP", new object[] { new HPathObject("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.Character.ServerCharacter')"), -1200 });

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
        public void Test_Send_Recieve_Damage()
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
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='CheatsPopupPanel']/*[@name='Cancel Button']", 30);
            api.Wait(3000);

            //Close how to play panel
            api.ClickObject(MouseButtons.LEFT, "/*[@name='BossRoomHudCanvas']/*[@name='HowToPlayPopupPanel']/*[@name='Confirmation Button']", 30);
            api.Wait(3000);

            //Validate player HP did change
            int endHp = api.GetObjectFieldValue<int>("//Player[@name='PlayerAvatar0']/fn:component('Unity.BossRoom.Gameplay.GameplayObjects.NetworkHealthState')/@HitPoints/@Value");
            Assert.Less(endHp, startingHp, "Player did not Recieve Damage!");

        }



        [OneTimeTearDown]
        public void Disconnect()
        {
            // Disconnect the GameDriver client from the agent
            api.StopEditorPlay();
            api.DisableHooks(HookingObject.ALL);
            api.Wait(2000);
            api.Disconnect();
            api.Wait(2000);
        }
    }
}