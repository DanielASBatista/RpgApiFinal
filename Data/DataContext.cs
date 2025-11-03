using Microsoft.EntityFrameworkCore;
using RpgApi.Models;
using RpgApi.Models.Enuns;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RpgApi.Utils;

namespace RpgApi.Data
{
    public class DataContext : DbContext
    {
        //ctor
        public DataContext
        (DbContextOptions<DataContext> options) : base(options)
        {

        }
        //prop 
        public DbSet<Personagem> TB_PERSONAGENS { get; set; }
        public DbSet<Arma> TB_ARMAS { get; set; }
        public DbSet<Usuarios> TB_USUARIOS { get; set; }
        public DbSet<Habilidade> TB_HABILIDADES { get; set; }
        public DbSet<PersonagemHabilidade> TB_PERSONAGENS_HABILIDADES { get; set; }
        public DbSet<Disputa> TB_DISPUTAS { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personagem>().ToTable("TB_PERSONAGENS");
            modelBuilder.Entity<Arma>().ToTable("TB_ARMAS");
            modelBuilder.Entity<Usuarios>().ToTable("TB_USUARIOS");
            modelBuilder.Entity<PersonagemHabilidade>().ToTable("TB_PERSONAGENS_HABILIDADE");
            modelBuilder.Entity<Habilidade>().ToTable("TB_HABILIDADE");
            modelBuilder.Entity<Disputa>().ToTable("TB_DISPUTAS");


            //1 pra N
            modelBuilder.Entity<Usuarios>().HasMany(e => e.Personagens)
                .WithOne(e => e.Usuario)
                .HasForeignKey(e => e.UsuarioId)
                .IsRequired(false);


            modelBuilder.Entity<Personagem>()
                .HasOne(e => e.Arma)
                .WithOne(e => e.Personagem).HasForeignKey<Arma>(e => e.PersonagemId)
                .IsRequired();

            modelBuilder.Entity<PersonagemHabilidade>().HasKey(ph => new { ph.PersonagemId, ph.HabilidadeId });
            modelBuilder.Entity<Habilidade>().HasData(
                new Habilidade() { Id = 1, Nome = "Adormecer", Dano = 39 },
                new Habilidade() { Id = 2, Nome = "Congelar", Dano = 41 },
                new Habilidade() { Id = 3, Nome = "Hipnotizar", Dano = 37 });

            modelBuilder.Entity<PersonagemHabilidade>().HasData(
                new PersonagemHabilidade() { PersonagemId = 1, HabilidadeId = 1 },
                new PersonagemHabilidade() { PersonagemId = 1, HabilidadeId = 2 },
                new PersonagemHabilidade() { PersonagemId = 2, HabilidadeId = 2 },
                new PersonagemHabilidade() { PersonagemId = 3, HabilidadeId = 2 },
                new PersonagemHabilidade() { PersonagemId = 3, HabilidadeId = 3 },
                new PersonagemHabilidade() { PersonagemId = 4, HabilidadeId = 3 },
                new PersonagemHabilidade() { PersonagemId = 5, HabilidadeId = 1 },
                new PersonagemHabilidade() { PersonagemId = 6, HabilidadeId = 2 },
                new PersonagemHabilidade() { PersonagemId = 7, HabilidadeId = 3 });

            modelBuilder.Entity<Personagem>().HasData
            (
                new Personagem() { Id = 1, Nome = "Frodo", PontosVida = 100, Forca = 17, Defesa = 23, Inteligencia = 33, Classe = ClasseEnum.Cavaleiro, UsuarioId = 1 },
                new Personagem() { Id = 2, Nome = "Sam", PontosVida = 100, Forca = 15, Defesa = 25, Inteligencia = 30, Classe = ClasseEnum.Cavaleiro, UsuarioId = 1 },
                new Personagem() { Id = 3, Nome = "Galadriel", PontosVida = 100, Forca = 18, Defesa = 21, Inteligencia = 35, Classe = ClasseEnum.Clerigo, UsuarioId = 1 },
                new Personagem() { Id = 4, Nome = "Gandalf", PontosVida = 100, Forca = 18, Defesa = 18, Inteligencia = 37, Classe = ClasseEnum.Mago, UsuarioId = 1 },
                new Personagem() { Id = 5, Nome = "Hobbit", PontosVida = 100, Forca = 20, Defesa = 17, Inteligencia = 31, Classe = ClasseEnum.Cavaleiro, UsuarioId = 1 },
                new Personagem() { Id = 6, Nome = "Celeborn", PontosVida = 100, Forca = 21, Defesa = 13, Inteligencia = 34, Classe = ClasseEnum.Clerigo, UsuarioId = 1 },
                new Personagem() { Id = 7, Nome = "Radagast", PontosVida = 100, Forca = 25, Defesa = 11, Inteligencia = 35, Classe = ClasseEnum.Mago, UsuarioId = 1 }
            );

            modelBuilder.Entity<Arma>().HasData
           (
             new Arma() { Id = 1, Nome = "Anti-Material Rifle", Dano = 35, PersonagemId = 1 },
             new Arma() { Id = 2, Nome = "Umbra", Dano = 33, PersonagemId = 2 },
             new Arma() { Id = 3, Nome = "Salame", Dano = 31, PersonagemId = 3 },
             new Arma() { Id = 4, Nome = "Blade of Chaos", Dano = 30, PersonagemId = 4 },
             new Arma() { Id = 5, Nome = "Marreita", Dano = 34, PersonagemId = 5 },
             new Arma() { Id = 6, Nome = "AK-43", Dano = 33, PersonagemId = 6 },
             new Arma() { Id = 7, Nome = "El machete", Dano = 32, PersonagemId = 7 }
           );

            //Início da criação do usuário padrão.
            Usuarios user = new Usuarios();
            Criptografia.CriarPasswordHash("123456", out byte[] hash, out byte[] salt);
            user.Id = 1;
            user.Username = "UsuarioAdmin";
            user.PasswordString = string.Empty;
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.Perfil = "Admin";
            user.Email = "seuEmail@gmail.com";
            user.Latitude = -23.5200241;
            user.Longitude = -46.596498;

            modelBuilder.Entity<Usuarios>().HasData(user);
            //Fim da criação do usuário padrão.

            //Define que se o perfil não for informado, o valor padrão será jogador
            modelBuilder.Entity<Usuarios>().Property(u => u.Perfil).HasDefaultValue("Jogadô");

            modelBuilder.Entity<Disputa>().HasKey(d => d.Id);//Indicação da chave primaria da entidade.
            //Abaixo fica o mapeamento do nome das colunas da tabela para as propriedades da classe.
            modelBuilder.Entity<Disputa>().Property(d => d.DataDisputa).HasColumnName("Dt_Disputa");
            modelBuilder.Entity<Disputa>().Property(d => d.AtacanteId).HasColumnName("AtacanteId");
            modelBuilder.Entity<Disputa>().Property(d => d.OponenteId).HasColumnName("OponenteId");
            modelBuilder.Entity<Disputa>().Property(d => d.Narracao).HasColumnName("Tx_Narracao");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings
                .Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}