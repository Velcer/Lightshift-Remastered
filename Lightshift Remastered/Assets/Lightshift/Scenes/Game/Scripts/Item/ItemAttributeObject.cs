using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ItemAttributeObject {

    public float speed;
    public float acceleration;
    public float agility;
    public float shield;
    public float health;

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(speed);
                writer.Write(acceleration);
                writer.Write(agility);
                writer.Write(shield);
                writer.Write(health);
            }
            return m.ToArray();
        }
    }

    public static ItemAttributeObject Desserialize(byte[] data)
    {
        ItemAttributeObject result = new ItemAttributeObject();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result.speed = (float)reader.ReadDouble();
                result.acceleration = (float)reader.ReadDouble();
                result.agility = (float)reader.ReadDouble();
                result.shield = (float)reader.ReadDouble();
                result.health = (float)reader.ReadDouble();
            }
        }
        return result;
    }
}
