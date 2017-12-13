using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoWPA.Model.Database.Model
{
    public class Professor
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual IList<Curso> Cursos { get; set; }
    }

    public class ProfessorMap: ClassMapping<Professor> 
    {
        public ProfessorMap()
        {
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Nome);

            Bag<Curso>(x => x.Cursos, m =>
            {
                m.Cascade(Cascade.All);
                m.Inverse(true);
                m.Lazy(CollectionLazy.NoLazy);
                m.Key(k => k.Column("idProfessor"));
            },
            o => o.OneToMany());
        }

    }
}
