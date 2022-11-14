namespace Patterns
{
    public class Bridge
    {
    }

    public abstract class Shape
    {
        public IRender Render { get; set; }
        public string Name { get; set; }

        public Shape(IRender render)
        {
            Render = render;
        }

        public override string ToString()
        {
            return $"Drawing {Name} as {Render.WhatTpRenderAs}";
        }
    }

    public class Triangle : Shape
    {
        public Triangle(IRender render) : base(render)
        { Name = "Triangle"; }
    }

    public class Square : Shape
    {
        public Square(IRender render) : base(render)
        { Name = "Square"; }
    }

    public class VectorRenderer : IRender
    {
        public string WhatTpRenderAs
            => "lines";
    }

    public class RasterRenderer : IRender
    {
        public string WhatTpRenderAs
            => "pixels";
    }

    public interface IRender
    {
        string WhatTpRenderAs { get; }
    }
}
