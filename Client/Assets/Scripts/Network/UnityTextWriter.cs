﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnityTextWriter : TextWriter
{
    private StringBuilder buffer = new StringBuilder();
    
    public override void Flush()
    {
        Debug.Log(buffer.ToString());
        buffer.Length = 0;
    }

    public override void Write(string value)
    {
        buffer.Append(value);
        if (value != null)
        {
            var len = value.Length;
            if (len > 0)
            {
                var lastChar = value[len - 1];
                if (lastChar == '\n')
                {
                    Flush();
                }
            }
        }
    }

    public override void Write(char value)
    {
        buffer.Append(value);
        if (value == '\n')
        {
            Flush();
        }
    }

    public override void Write(char[] value, int index, int count)
    {
        Write(new string(value, index, count));
    }

    public override void WriteLine(string value)
    {
        buffer.Append(value);
        Flush();
    }

    public override Encoding Encoding
    {
        get { return Encoding.Default; }
    }
}