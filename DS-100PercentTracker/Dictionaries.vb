Public Class Dictionaries

    'Dictionary for the starting items in Asylum. Key is the starting class, values are the starting item flags
    Public Shared startingClassItems As Dictionary(Of PlayerStartingClass, Array)

    'Dictionary for the dropped item flags of each NPC. Key is the dead flag for each NPC, values are the dropped item flags
    Public Shared npcDroppedItems As Dictionary(Of Integer, Array)

    'Dictionary for treasure locations that have multiple pickups/event flags associated with it. Key is the first flag, values are the remaining flags
    Public Shared sharedTreasureLocationItems As Dictionary(Of Integer, Array)

    Shared Sub New()
        startingClassItems = New Dictionary(Of PlayerStartingClass, Array)

        startingClassItems.Add(PlayerStartingClass.Warrior, {51810110, 51810100})
        startingClassItems.Add(PlayerStartingClass.Knight, {51810130, 51810120})
        startingClassItems.Add(PlayerStartingClass.Wanderer, {51810150, 51810140})
        startingClassItems.Add(PlayerStartingClass.Thief, {51810170, 51810160})
        startingClassItems.Add(PlayerStartingClass.Bandit, {51810190, 51810180})
        startingClassItems.Add(PlayerStartingClass.Hunter, {51810210, 51810200, 51810220})
        startingClassItems.Add(PlayerStartingClass.Sorcerer, {51810240, 51810230, 51810250})
        startingClassItems.Add(PlayerStartingClass.Pyromancer, {51810270, 51810260, 51810280})
        startingClassItems.Add(PlayerStartingClass.Cleric, {51810300, 51810290, 51810310})
        startingClassItems.Add(PlayerStartingClass.Deprived, {51810330, 51810320})

        npcDroppedItems = New Dictionary(Of Integer, Array)

        'Friendly NPCs
        npcDroppedItems.Add(1322, {51010990}) 'Andre
        npcDroppedItems.Add(1342, {51300990, 51300991}) 'Vamos
        npcDroppedItems.Add(1097, {50007030, 50007031}) 'Logan
        npcDroppedItems.Add(1284, {51400980}) 'Eingyi
        npcDroppedItems.Add(1872, {50000520}) 'Elizabeth
        npcDroppedItems.Add(1115, {50006041, 50006040}) 'Griggs
        npcDroppedItems.Add(1823, {50000510, 50000511}) 'Gough
        npcDroppedItems.Add(1034, {50006010}) 'Darkling Lady
        npcDroppedItems.Add(1842, {51210990}) 'Chester
        npcDroppedItems.Add(1702, {11607020, 50006371}) 'Oswald
        npcDroppedItems.Add(1628, {50006320, 50006321}) 'Patches
        npcDroppedItems.Add(1198, {50006080, 50006081}) 'Petrus
        npcDroppedItems.Add(1295, {50000300}) 'Quelana
        npcDroppedItems.Add(1177, {50006070, 50006072}) 'Rhea
        npcDroppedItems.Add(1604, {50006310, 50006311}) 'Shiva
        npcDroppedItems.Add(1764, {50006420, 50006421}) 'Shiva's Bodyguard
        npcDroppedItems.Add(1402, {51010960}) 'Undead Merchant (Male)
        npcDroppedItems.Add(1272, {51400990}) 'Fair Lady
        npcDroppedItems.Add(1864, {50000501, 50000500}) 'Ciaran
        npcDroppedItems.Add(1513, {50006280, 50000070}) 'Siegmeyer
        npcDroppedItems.Add(11200818, {50008000, 50008001}) 'Pharis


        npcDroppedItems.Add(11210021, {51210910}) 'Sif rescued in DLC
        npcDroppedItems.Add(11210681, {51210921}) 'Carving Mimic in DLC
        npcDroppedItems.Add(11210680, {51210981}) 'Crest Key Mimic in DLC
        npcDroppedItems.Add(800, {51410990}) 'Sunlight Maggot Chaos Firebug
        npcDroppedItems.Add(1062, {51410990}) 'Hollowed Oscar


        sharedTreasureLocationItems = New Dictionary(Of Integer, Array)

        sharedTreasureLocationItems.Add(50001030, {50001031}) 'Dingy Set + Black Eye Orb
        sharedTreasureLocationItems.Add(51010380, {51010381}) 'Thief Set + Target Shield
        sharedTreasureLocationItems.Add(51010510, {51010511}) 'Sorcerer Set + Sorcerer Catalyst
        sharedTreasureLocationItems.Add(51200020, {51200021}) 'Hunter Set + Longbow
        sharedTreasureLocationItems.Add(51200140, {51200141, 51200142}) 'Divine Ember + Watchtower Basement Key + Homeward Bone
        sharedTreasureLocationItems.Add(51300190, {51300191}) 'Cleric Set + Mace
        sharedTreasureLocationItems.Add(51400190, {51400191}) 'Crimson Set + Tin Banishment Catalyst
        sharedTreasureLocationItems.Add(51400270, {51000271}) 'Poison Mist + Pyromancer Set
        sharedTreasureLocationItems.Add(51400310, {51400311}) 'Wanderer Set + Falchion
        sharedTreasureLocationItems.Add(51500060, {51500061}) 'Black Sorcerer Set + Hush
        sharedTreasureLocationItems.Add(51510070, {51510071}) 'Black Iron Set + Greatsword/Black Iron Greatshield
        sharedTreasureLocationItems.Add(51600220, {51600221}) 'Bandit Set + Spider Shield
        sharedTreasureLocationItems.Add(51600360, {51600361}) 'Witch Set + Beatrice's Catalyst
        sharedTreasureLocationItems.Add(51700070, {51700071}) 'Maiden Set + White Seance Ring
        sharedTreasureLocationItems.Add(51700640, {51700641}) 'Sage Set + Logan's Catalyst


    End Sub

End Class
