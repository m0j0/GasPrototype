using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Test.PipeFlowScheme
{
    class FlowTests
    {
        //      d1              d2
        //      |               |
        //      |               |
        //      v7              v8
        //      |               |
        //      |               |
        // -----c1----     -----c4----
        // |    |    |     |    |    |
        // |    |    |     |    |    |
        // v1   v2   v3    v4   v5   v6
        // |    |    |     |    |    |
        // |    |    |     |    |    |
        // |    -----c2----c3----    |
        // |         |               |
        // s1        s2              s3

        // s1    s2    s3    s4    s5    s6    s7
        // |     |     |     |     |     |     |
        // |     |     |     |     |     |     |
        // v1    v2    v3    v4    v5    v6    v7
        // |     |     |     |     |     |     |
        // |     |     |     |     |     |     |
        // ------c1----c2----c3----c4----c5-----
        //             |
        //             |
        //             d1
    }
}
