using System.Numerics;

namespace gk2.Drawing.NormalMap {
    public abstract class NormalMap {
        public abstract Vector3 GetVector(int x, int y);
    }
}
