using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System.Collections.Generic;

namespace EstudoWPA.Model.Database.Model
{
    public class Aluno
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual Curso Curso { get; set; }
    }

    public class AlunoMap : ClassMapping<Aluno>
    {
        public AlunoMap()
        {
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Nome);
            ManyToOne<Curso>(x => x.Curso, m => m.Column("idAluno"));
        }
    }
}
