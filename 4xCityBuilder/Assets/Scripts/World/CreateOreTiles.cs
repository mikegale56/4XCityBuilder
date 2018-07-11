using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

public class CreateOreTiles {

    public Texture2D oreTexture;
    public Texture2D rockTexture;

    public void RockOreMix(Dictionary<string, byte> undergroundValueDictionary, Dictionary<string, byte> stoneValueDictionary, List<List<Tile>> undergroundTiles)
    {
        int i, j;

        // Load the texture for Ore
        oreTexture   = Resources.Load("Textures/OreBase")   as Texture2D;

        // Create the colors and names for the different ores
        List<Color> colorList = new List<Color>();
        List<string> nameList = new List<string>();
        nameList.Add("CopperOre");
        colorList.Add(new Color(55F / 255F, 109F / 255F, 75F / 255F));
        nameList.Add("TinOre");
        colorList.Add(new Color(73F / 255F, 78F / 255F, 84F / 255F));
        nameList.Add("SilverOre");
        colorList.Add(new Color(242F / 255F, 247F / 255F, 245F / 255F));
        nameList.Add("IronOre");
        colorList.Add(new Color(202F / 255F, 114F / 255F, 84F / 255F));
        nameList.Add("GoldOre");
        colorList.Add(new Color(242F / 255F, 203F / 255F, 62F / 255F));

        byte[] bytes;
        string m_Path = Application.dataPath;

        // For each ore
        for (int n=0; n<nameList.Count; n++)
        {
            //Debug.Log("Name List " + n.ToString() + ", " + nameList[n]);
            // Copy the textures
            Texture2D newOreTex = new Texture2D(32, 32);
            newOreTex.SetPixels32(oreTexture.GetPixels32());
            // Override the black pixels with the specific ore color
            for (i = 0; i < oreTexture.height; i++)
            {
                for (j = 0; j < oreTexture.width; j++)
                {
                    Color c = oreTexture.GetPixel(i, j);
                    // If it is a black pixel, replace it
                    if (c == Color.black)
                        newOreTex.SetPixel(i, j, colorList[n]);
                }
            }
            // Merge in each stone
            Texture2D mergedTex = new Texture2D(32, 32);
            foreach (KeyValuePair<string, byte> entry in stoneValueDictionary)
            {
                //Debug.Log("Stone " + entry.Key);
                MergeTextures(mergedTex, undergroundTiles[0][entry.Value].sprite.texture, newOreTex, undergroundTiles[0][entry.Value].color);
                bytes = mergedTex.EncodeToPNG();
                File.WriteAllBytes(m_Path + "/Resources/Textures/Ore/" + nameList[n] + entry.Key + ".png", bytes);
                Tile newTile = ScriptableObject.CreateInstance<Tile>();
                var newSprite = Resources.Load("/Textures/Ore/" + nameList[n] + entry.Key + ".png") as Sprite;
                newTile.sprite = newSprite;
                if (!undergroundValueDictionary.ContainsKey(nameList[n]))
                    undergroundValueDictionary.Add(nameList[n], (byte)(undergroundValueDictionary.Count));
            }
        }
    }

    public void MergeTextures(Texture2D newTex, Texture2D baseTex, Texture2D overwriteTex, Color baseTileColor)
    {
        // Overwrite texture in baseTex when overwriteTex has a non-trivial color
        for (int i = 0; i < oreTexture.height; i++)
            for (int j = 0; j < oreTexture.width; j++)
                // Trivial color is nearly white
                if (overwriteTex.GetPixel(i, j).r >= 0.93 && overwriteTex.GetPixel(i, j).g >= 0.93 && overwriteTex.GetPixel(i, j).b >= 0.93)
                {
                    newTex.SetPixel(i, j, baseTex.GetPixel(i, j)*baseTileColor);
                    //Debug.Log("Stone pixel " + i.ToString() + "," + j.ToString());
                }
                else
                {
                    newTex.SetPixel(i, j, overwriteTex.GetPixel(i, j));
                    /*Debug.Log("Overwrite pixel " + i.ToString() + "," + j.ToString() + " with color " +
                        overwriteTex.GetPixel(i, j).r.ToString() + "," + overwriteTex.GetPixel(i, j).g.ToString() + "," +
                        overwriteTex.GetPixel(i, j).b.ToString()); */
                }
    }

}


