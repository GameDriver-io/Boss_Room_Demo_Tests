#!/bin/sh
# This is intended to run the Boss_Room_Tests test using GameDriver

mono ./nunit3-console.exe ../../../Boss_Room_Tests/bin/Debug/Boss_Room_Tests.dll --testparam:Mode=standalone --testparam:testHost=localhost
