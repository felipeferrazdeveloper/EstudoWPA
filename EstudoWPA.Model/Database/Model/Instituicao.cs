using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoWPA.Model.Database.Model
{
    public class Instituicao
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual IList<Curso> Cursos { get; set; }
    }
    
    public class InstituicaoMap : ClassMapping<Instituicao>
    {
        public InstituicaoMap()
        {
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Id);


            Bag<Curso>(x => x.Cursos, m =>
            {
                m.Cascade(Cascade.All);
                m.Inverse(true);
                m.Lazy(CollectionLazy.NoLazy);
                m.Key(k => k.Column("idInstituicao"));
            },
            o => o.OneToMany());
        }
    }
}
