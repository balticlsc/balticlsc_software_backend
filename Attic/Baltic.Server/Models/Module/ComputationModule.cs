#pragma warning disable 1591
using System;
using System.Collections.Generic;
using Baltic.Server.Models.Resources;

namespace Baltic.Server.Models.Module
{
    public class ComputationModule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ForkId { get; set; } //parent
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Icon { get; set; }
        public int Rate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int TimesUsed { get; set; }
        public bool IsBenchmark { get; set; }
        public string Type { get; set; }
        public IEnumerable<string> Keywords { get; set; }
        public string ProblemClass { get; set; }
        public IEnumerable<ResourceConstraints> ExternalResourcesConstraints { get; set; }
        public bool OpenSource { get; set; }
    }
}

//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//          |\_                \|\||             
//        -' | `.             -- ||||/       
//       /7      `-._        /7   |||||/            
//      /            `-.____/    |||||||/`-.____________
//      \-'_                \-' |||||||||               `-._
//       -- `-.              -/||||||||\                `` -`.
//             |\              /||||||\             \_  |   `\\
//             | \  \_______...-//|||\|________...---'\  \    \\
//             |  \  \            ||  |  \ ``-.__--. | \  |    ``-.__--.
//             |  |\  \          / |  |\  \   ``---'/ / | |       ``---'
//           _/  / _|  )      __/_/  / _|  )     __/ / _| |
//          /,__/ /,__/      /,_/,__/_/,__/     /,__/ /,__/          