using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuItem
{
    public string entree;
    public string side;
    public string vegetable;
    public string sauce;
}

public class MenuDatabase : MonoBehaviour
{
    public Dictionary<Rank, MenuItem> Map { get; private set; }

    private void Awake()
    {
        Map = new Dictionary<Rank, MenuItem>
        {
            { Rank.Ace, new MenuItem {
                entree = "A5 Miyazaki Wagyu",
                side = "Gold-Leaf Saffron Risotto",
                vegetable = "White Asparagus with Truffle",
                sauce = "Perigord Truffle Jus"
            }},
            { Rank.King, new MenuItem {
                entree = "Pan-Seared Squab",
                side = "Forbidden Black Rice",
                vegetable = "Heirloom Baby Beets",
                sauce = "Foie Gras Emulsion"
            }},
            { Rank.Queen, new MenuItem {
                entree = "Miso-Glazed Black Cod",
                side = "Thousand Layer Potatoes",
                vegetable = "Charred Romanesco",
                sauce = "Yuzu Ginger Reduction"
            }},
            { Rank.Jack, new MenuItem {
                entree = "Duck Confit",
                side = "Wild Mushroom Risotto",
                vegetable = "Garlic Green Beans",
                sauce = "Red Wine Orange Reduction"
            }},
            { Rank.Ten, new MenuItem {
                entree = "Dry-Aged Filet Mignon",
                side = "Potatoes au Gratin",
                vegetable = "Creamed Spinach",
                sauce = "Peppercorn Sauce"
            }},
            { Rank.Nine, new MenuItem {
                entree = "Blackened Salmon",
                side = "Rice Pilaf",
                vegetable = "Honey Glazed Carrots",
                sauce = "Dill Sauce"
            }},
            { Rank.Eight, new MenuItem {
                entree = "Grilled Lamb Chops",
                side = "Lemon Herb Orzo",
                vegetable = "Roasted Brussel Sprouts",
                sauce = "Mint Chimichurri"
            }},
            { Rank.Seven, new MenuItem {
                entree = "Smoked Brisket",
                side = "Macaroni & Cheese",
                vegetable = "Collard Greens",
                sauce = "Bourbon BBQ Sauce"
            }},
            { Rank.Six, new MenuItem {
                entree = "Beer-Battered Cod",
                side = "Thick-Cut Chips",
                vegetable = "Pickled Veg",
                sauce = "Tartar Sauce"
            }},
            { Rank.Five, new MenuItem {
                entree = "Country Fried Steak",
                side = "Instant Mash",
                vegetable = "Steamed Broccolini",
                sauce = "Sawmill White Gravy"
            }},
            { Rank.Four, new MenuItem {
                entree = "Chicken Nuggets",
                side = "Seedy Oil French Fries",
                vegetable = "Shredded Iceberg Lettuce",
                sauce = "BBQ Sauce"
            }},
            { Rank.Three, new MenuItem {
                entree = "Boiled Eggs",
                side = "Instant Ramen Noodles",
                vegetable = "Frozen Peas",
                sauce = "Sriracha"
            }},
            { Rank.Two, new MenuItem {
                entree = "Gas Station Glizzy",
                side = "Glizzy Bun",
                vegetable = "Rehydrated Onion Bits",
                sauce = "Yellow Mustard Packet"
            }},
        };
    }

    // Convenience getters (used by DishSpawner)
    public string GetEntree(Rank r) => Map[r].entree;
    public string GetSide(Rank r) => Map[r].side;
    public string GetVegetable(Rank r) => Map[r].vegetable;
    public string GetSauce(Rank r) => Map[r].sauce;
}