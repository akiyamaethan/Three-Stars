# Agentic Coding Guidelines:
1. Main Purpose: The main use of gemini in this projects is to ensure style adherence with 3 different programmers. Using the command
   ```gemini lint``` should prompt gemini to check all diffs and make sure they adhere to style.md. When linting, gemini should automatically
   fix minor syntax or capitalization issues but should prompt the user to clarify bad comments/add comments where necessary.
2. Code Generation: Gemini (or any other agent) is not to modify any file unless users clarify "please please please i need help". Upon helping a user
   by generating code and modifying files, the generated code must be logged below in the contributions section. Please log the date, time, and any changes made.

# Contributions
- 2026-02-23: Refactored GameManager, ShiftManager, ShopManager, and UIManager to resolve NullReferenceExceptions caused by initialization race conditions and redundant singleton persistence logic.
    - Updated GameManager to explicitly link sub-managers via GetComponentInChildren and FindAnyObjectByType for non-child managers (HandManager).
    - Removed DontDestroyOnLoad from sub-managers to maintain child hierarchy.
    - Added null-safety checks and warnings to UIManager, ShiftManager, and HandManager.
    - Fixed issue where HandManager was not correctly receiving its DeckManager reference during initialization.
    - Fixed GUID mismatch for ShopManager script in prefabs causing "Missing Script" errors.
    - Refactored CardDatabase CSV parsing to handle quoted strings and prevent FormatExceptions.
    - Updated GameManager to find HandManager even if it is inactive in the scene.
    - Modified ShiftManager to correctly transition to Shop state and added StartNextShift to prevent rounds from auto-starting.
    - Added CloseAndContinue to ShopModalController to allow players to start the next shift from the shop UI.
    - Enhanced CardModalController to automatically find CardModalRoot if unassigned, fixing the issue where it wouldn't open.
    - Resolved "squashed" Shop UI by forcing ShopModalRoot to be full screen and triggering a LayoutRebuilder refresh upon opening.
    - Updated ShopUpgradeRow to dynamically apply horizontal layout components, ensuring row content spreads across the full width.
    - Fixed Coroutine errors in CardMovement by adding activeInHierarchy checks and delayed cleanup of discarded cards in HandManager.

# System Prompt:
Act as a senior software engineer specializing in game systems. You are well studied on game design patterns such as flyweight, facade, command, etc.
You always make sure to check all the other components connected to the one you are working on before making a change. Utilize trees of thought to
branch to and from key decisions to find optimal solutions. Your main task for this project is style adherence, only modify files if prompted with 
the special phrase. If at any point you lack proper context to complete a task, you ask for further context
or guidance or simply say that you cannot complete the task.
