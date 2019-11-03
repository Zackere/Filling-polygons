using System;
using System.Numerics;

namespace gk2.Drawing.NormalMap {
    public abstract class NormalMap : IDisposable {
        public abstract void Dispose();
        public abstract Vector3 GetVector(int x, int y);
    }
}
