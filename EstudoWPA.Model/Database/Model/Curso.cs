using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoWPA.Model.Database.Model
{
    public class Curso
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual Instituicao Instituicao { get; set; }
        public virtual IList<Aluno> Alunos { get; set; }
        public virtual Professor Professor { get; set; }
    }

    public class CursoMap : ClassMapping<Curso>
    {
        public CursoMap()
        {
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Nome);

            ManyToOne<Instituicao>(x => x.Instituicao, m => m.Column("idInstituicao"));
            ManyToOne<Professor>(x => x.Professor, m => m.Column("idProfessor"));
            Bag<Aluno>(x => x.Alunos, m =>
            {
                m.Cascade(Cascade.All);
                m.Inverse(true);
                m.Lazy(CollectionLazy.NoLazy);
                m.Key(k => k.Column("idAluno"));
            },
            o => o.OneToMany());
        }
    }
}
