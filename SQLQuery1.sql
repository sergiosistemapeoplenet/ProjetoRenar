select * from Renar.Unidade
select * from Renar.Regiao

select * from Acesso.Usuario

update Acesso.Usuario
set FlagPrimeiroAcesso = 1
where IDUsuario = 42

insert into Renar.Unidade(NomeUnidade, CNPJ, IDRegiao, Endereco, FlagAtivo, Cep, HorarioFuncionamento, QuantidadeUso, SerialImpressora, NomeContato, EmailContato, FlagImprimeCodigoBarra)
values('Mané Recreio', '46678364000126', 1, 'Rua Carlos Galhardo, 85 - Recreio do Bandeirantes RJ', 1, '22750440', '11h às 22h', 1, NULL, 'Pamela', 'Recreio@botecomane.com.br', 0)