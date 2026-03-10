# Three-Stars
Cooking Themed Deckbuilding Roguelike inspired by Balatro. Designed by Ethan Akiyama. Helped by Humza Saulat and Manvir Sohi

Vision Statement:

	Three Stars is a heavily cooking themed roguelike deckbuilder. Inspired by Balatro, the player is dealt cards and must use and discard them to make 4-card hands that score enough to move onto the next shift. Between shifts, the player can purchase upgrades that modify the way cards score as well as chef cards which drastically change the way cards score. Chef cards are placed in 1 of 2 slots next to the player's hand, activating left to right as the final step of scoring a hand. 

Overview:
Three Stars is played with 52 standard cards (re-suited to entree, side, veg, sauce) plus a separate deck of chef cards that appear in the shop and randomly throughout the deck. The game is played in rounds (shifts). The core loop of the game follows this pattern: (play a shift -> access the shop) x 4 -> boss(critic) shift. Each shift, 7 cards are dealt onto the mat and the player has 3 discards and 3 plays to meet the required score to move to the next shift. Plays and discards consist of up to 4 cards. After playing or discarding, cards are drawn from the deck until there are 7 on the mat again. During each shift, discarded cards are removed from the deck. The deck is reset after each shift. 

Chef Cards:
	Chef Cards are the core engine of the game. They can grant incredible bonuses to the player but they only last X amount of shifts before expiring. Additionally, 1 random Chef Card is shuffled into the deck of cards each shift. If at any point the player draws it, it is immediately activated and subsequently discarded upon playing the next hand, ignoring the chef card limit. Chef Cards can be upgraded via the shop. These upgrades are capable of increasing the amount of shifts each Chef Card lasts, the amount of Chef Card slots, the amount of times each Chef Card activates per hand played, and the amount of Chefs randomly shuffled into the deck each shift.

Hand scoring/names:

Hands
In game name
Score
ex
High Card
A la carte
X 1
AE 2Sd 3V 5V
Rainbow
Balanced Meal
X 1.15
AE 2Sd 3V 6S
Pair
Pairing
x 1.25
AE ASd 3V 5V
2 Pair
Split Plate
x 1.35
AE ASd KV KS
Trips
Set
x 1.45
AE ASd AV KS
Straight
Buffet
x 1.75
JE QSd KV AS
Flush
Flight
x 2
AE JE 8E 5E
Quads
Perfect Meal
x 5
AE ASd AV AS
Straight Flush
Buffet Flight
x 10
5E 6E 7E 8E
Royal Flush
Grand Flight
x 20
JE QE KE AE


Card Levelling:
    Each individual rank and suit are levelled up independently and a card benefits from both levels when it is played. This means if sauces are level 2 and jacks are level 2, JS gains 2 bonus pips (one from sauces and one from jacks). Below is the levelling schematic for suits and ranks: 
Level 1–5: Requires 1 play to level up. 
Level 6–15: Requires 3 plays to level up.
Level 16+: Requires 10 plays to level up. 
    Every time a hand is played, it is first scored before ranks/suits gain a play. So after scoring, each unique rank and suit in that hand gain 1 play. That means the hand JE QE KE AE will add 1 play to J, Q, K, A ranks and the E suit. Each level adds 1 pip value internally.

Round Progression:
The first shift required score is 100 and it, as well as other normal shifts, awards 10 coins to spend in the shop upon completion. The score threshold will increase per shift at a rate TBD. Boss shifts award 40 coins. Coins are kept between shifts meaning they can be saved up and spent later or spent as soon as they are acquired.

Shop:
Each instance of the shop contains 3 random upgrade cards for purchase as well as two random chef cards, available at no cost to the player. Upgrade cards are capable of increasing all of, but not limited to, the following:
Amount of pips scored for ranks (ex: 2s)
Amount of pips scored for suits (ex: vegetables)
Amount of pips scored for hands
Multipliers for hands
Removing/adding cards to the deck for the duration of the run
Shop refreshes and discount
Chef card count in the deck and chef card slot count
Chef card durability 
Chef card activations per hand
The upgrades will cost 5-10 coins. These options (5 total) will not refresh until the next instance of the shop (player gets at max 3 upgrades + 2 chefs per shift). These will be randomly drawn from using a weighted grammar with the super powerful passive upgrades (like giving the player 1 shop refresh per shift) and legendary chefs at around 1%. The shop can be upgraded via the shop as mentioned above, increasing the amount of refreshes available to the player and decreasing shop prices.

Scoring:
	When a hand is played, each card in the played hand should have in internal tally of their individual pips:
Base pips + extra pips from current level of suit + extra pips from current level of rank + extra pips from any upgrade cards
It then adds any bonus pips that an active chef card may provide before adding its total pips to the total. The total pips for all cards in the played hand are then evaluated for hand multipliers. The game algorithmically chooses the best scoring multiplier and applies it. Next, each chef card is iterated again for multipliers and if applicable, they are applied x amount of times depending on the players stats. Finally, each card rolls for a crit. If a card crits, it multiplies the total hand score by 1.25x. So if 2 out of the 4 player’s cards crit, the hand will be multiplied by 1.25 twice after the chef card effects. 

Quads:
	Quads grant immediate access one of five upgrade cards that manipulate the deck: Remove a low card from the deck, add a low card to the deck, remove a high card from the deck, add a high card to the deck, magically change one card in the deck into any card of your choosing for the rest of the match.

Card Modal:
The current level for each card and suit as well as the current multiplier for each hand should be accessible at any time during gameplay in a popup modal called “cards”. The cards modal will contain the table of hands on the left of the modal and a table of each rank and suit (17 total entries) and its current ‘level’ on the right. The table of hands will update the pip values multiplier as it is increased with upgrades, chef cards, etc. If the player hovers on the name of a hand, a small popup showing an example of the hand will be displayed.



# Development Tools:
- Unity 2022.3.62f2
- Github Desktop
- Gemini Cli (for style adherence and stuck moments)

# Gemini Install
Part 1: Activate Google AI Student Account<br>
## Make sure not to use your ucsc email or it will not allow you to use the CLI<br>

1. Verify Eligibility: Go to the Google One Student Page: https://gemini.google/students/

2. Verify via SheerID: You will be prompted to verify your student status. Use your school (.edu) email or upload proof of enrollment.

3. Redeem the Offer: Once SheerID confirms your status, return to Google One and select Get Student Offer.

4. Complete Purchase: You will need to add a payment method, but the cost will be $0 for the first 12 months.

Part 2: Install Gemini CLI
1. Check/Install Node.js
Open your terminal and check your version:
```Bash
node -v
```
If you don't have it, download it from nodejs.org: https://nodejs.org/en

2. Install the CLI Globally
Run the following command to install the Gemini CLI:
```Bash
npm install -g @google/gemini-cli
```
3. Launch and Authenticate
Start the tool by typing:
```Bash
gemini
```
