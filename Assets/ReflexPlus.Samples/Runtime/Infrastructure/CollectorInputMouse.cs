using ReflexPlus.Sample.Application;
using UnityEngine;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class CollectorInputMouse : ICollectorInput
    {
        public Vector2 Get()
        {
            return new Vector2
            {
                x = Input.GetAxis("Mouse X"),
                y = Input.GetAxis("Mouse Y")
            };
        }
    }
}