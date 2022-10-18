# ALIENS_AFTER_AVA_2022
This is the subtree repository for only the C# scripts of my Steam game, Aliens After Ava!

Much of how the game shares code across files is by using the PropertySO (PSO) abstract class I designed by taking advantage of ScriptableObjects in Unity. A PropertySO class will always be for a custom class type (or a standard one that I defined such as int or float), and allows for delegations to be easily hooked up and visible within Unity.

A few singletons are also included, but their use was able to be minimized due to the effectiveness of PropertySOs.

The audio system is also custom made for this game (initially co-written with audio designer, Sean Porio), which was later refactored to take advantage of the PSOs and audio wrappers created for the game. The wrappers provide a hierarchy system of mixing and matching sound effects and groups of sound effects, and being able to adjust each of the wrappers' volume and delay.

If you would like to play the game, feel free to check out the <a href="https://store.steampowered.com/app/1995780/Aliens_After_Ava/" target="_blank">Steam store page here</a>, or download a standalone version at the <a href="https://www.alejandrohervella.com/aliensafterava" target="_blank">official site here.</a> 

If you would like access to the full Unity project repo, please don't hesitate to email me at ahervella@me.com!
