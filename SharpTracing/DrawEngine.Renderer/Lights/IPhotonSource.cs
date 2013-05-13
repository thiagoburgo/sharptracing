using System.Collections.Generic;
using DrawEngine.Renderer.PhotonMapping;

namespace DrawEngine.Renderer.Lights {
    public interface IPhotonSource {
        int MaxPhotons { get; set; }
        IEnumerable<Photon> GeneratePhotons();
    }
}