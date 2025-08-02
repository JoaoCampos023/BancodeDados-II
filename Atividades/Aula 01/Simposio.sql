-- Criação do banco de dados
CREATE DATABASE Simposio;
GO

-- Tabelas Principais
CREATE TABLE Pessoa(
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    cpf VARCHAR(11) NOT NULL UNIQUE,
    nome_completo VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    telefone VARCHAR(15) NOT NULL UNIQUE,
    sexo CHAR(1) NOT NULL CHECK (sexo IN ('M','F')),
    data_nascimento DATE NOT NULL,
    data_cadastro DATETIME DEFAULT GETDATE(),
    ultima_atualizacao DATETIME DEFAULT GETDATE(),
    cod_universitario INT NULL,
    eh_universitario BIT NOT NULL DEFAULT 0,
    tipo_pessoa VARCHAR(20) DEFAULT 'Participante' NOT NULL CHECK (tipo_pessoa IN ('Participante', 'Palestrante', 'Organizador')),
    CONSTRAINT chk_cod_universitario CHECK((eh_universitario = 1 AND cod_universitario IS NOT NULL) OR (eh_universitario = 0 AND cod_universitario IS NULL))
);
GO

CREATE TABLE Tema(
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    descricao TEXT NOT NULL,
    area_conhecimento VARCHAR(255) NOT NULL,
    data_criacao DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Comissao(
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    nome VARCHAR(200) NOT NULL,
    id_tema INT NOT NULL,
    data_criacao DATETIME DEFAULT GETDATE(),
    descricao TEXT,
    CONSTRAINT Fk_comissao_tema FOREIGN KEY (id_tema) REFERENCES Tema(id)
);
GO

CREATE TABLE Minicurso(
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    titulo VARCHAR(255) NOT NULL,
    data_evento DATE NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fim TIME NOT NULL,
    id_tema INT NOT NULL,
    CONSTRAINT FK_minicurso_tema FOREIGN KEY (id_tema) REFERENCES Tema(id),
    CONSTRAINT chk_horario_minicurso CHECK (hora_inicio < hora_fim)
);
GO

CREATE TABLE Artigo(
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    avaliacao VARCHAR(20) DEFAULT 'Pendente' CHECK (avaliacao IN ('Aprovado','Reprovado','Pendente')),
    titulo VARCHAR(200) NOT NULL,
    data_submissao DATE NOT NULL,
    data_avaliacao DATE NULL,
    nota_avaliacao INT NULL,
    id_comissao INT NOT NULL,
    id_tema INT NOT NULL,
    CONSTRAINT Fk_artigo_comissao FOREIGN KEY (id_comissao) REFERENCES Comissao(id),
    CONSTRAINT Fk_artigo_tema FOREIGN KEY (id_tema) REFERENCES Tema(id),
    CONSTRAINT chk_nota_avaliacao CHECK (nota_avaliacao IS NULL OR (nota_avaliacao >= 0 AND nota_avaliacao <= 10))
);
GO

CREATE TABLE Palestra(
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    titulo VARCHAR(255) NOT NULL,
    data_evento DATE NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fim TIME NOT NULL,
    id_tema INT NOT NULL,
    CONSTRAINT Fk_palestra_tema FOREIGN KEY (id_tema) REFERENCES Tema(id),
    CONSTRAINT chk_horario_palestra CHECK (hora_inicio < hora_fim)
);
GO

CREATE TABLE Simposio(
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    titulo VARCHAR(200) NOT NULL,
    descricao TEXT,
    universidade VARCHAR(255) NOT NULL,
    data_inicio DATE NOT NULL,
    data_fim DATE NOT NULL,
    id_orador_principal INT NOT NULL,
    local_simposio VARCHAR(255) NOT NULL,
    capacidade INT,
    CONSTRAINT Fk_orador_simposio FOREIGN KEY (id_orador_principal) REFERENCES Pessoa(id),
    CONSTRAINT chk_data_simposio CHECK (data_inicio <= data_fim)
);
GO

-- Tabelas de Relacionamento
CREATE TABLE Comissao_Pessoa(
    id_pessoa INT NOT NULL,
    id_comissao INT NOT NULL,
    PRIMARY KEY (id_pessoa, id_comissao),
    CONSTRAINT Fk_comissao_id_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id),
    CONSTRAINT Fk_comissao_id_comissao FOREIGN KEY (id_comissao) REFERENCES Comissao(id)
);
GO

CREATE TABLE Inscrito_Simposio(
    id_evento INT NOT NULL,
    id_pessoa INT NOT NULL,
    data_inscricao DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (id_pessoa, id_evento),
    CONSTRAINT Fk_inscrito_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id),
    CONSTRAINT Fk_inscrito_simposio FOREIGN KEY (id_evento) REFERENCES Simposio(id)
);
GO

CREATE TABLE Minicurso_Simposio(
    id_simposio INT NOT NULL,
    id_minicurso INT NOT NULL,
    PRIMARY KEY (id_simposio, id_minicurso),
    CONSTRAINT Fk_minicurso_simposio FOREIGN KEY (id_simposio) REFERENCES Simposio(id),
    CONSTRAINT Fk_simposio_minicurso FOREIGN KEY (id_minicurso) REFERENCES Minicurso(id)
);
GO

CREATE TABLE Artigo_Simposio(
    id_simposio INT NOT NULL,
    id_artigo INT NOT NULL,
    PRIMARY KEY (id_simposio, id_artigo),
    CONSTRAINT Fk_artigo_simposio FOREIGN KEY (id_simposio) REFERENCES Simposio(id),
    CONSTRAINT Fk_simposio_artigo FOREIGN KEY (id_artigo) REFERENCES Artigo(id)
);
GO

CREATE TABLE Palestra_Simposio(
    id_simposio INT NOT NULL,
    id_palestra INT NOT NULL,
    PRIMARY KEY (id_simposio, id_palestra),
    CONSTRAINT Fk_simposio_palestra FOREIGN KEY (id_simposio) REFERENCES Simposio(id),
    CONSTRAINT Fk_palestra_simposio FOREIGN KEY (id_palestra) REFERENCES Palestra(id)
);
GO

CREATE TABLE Organizador_Simposio (
    id_pessoa INT NOT NULL,
    id_simposio INT NOT NULL,
    PRIMARY KEY (id_pessoa, id_simposio),
    CONSTRAINT Fk_organizador_simposio_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id),
    CONSTRAINT Fk_organizador_simposio_simposio FOREIGN KEY (id_simposio) REFERENCES Simposio(id)
);
GO

CREATE TABLE Autor_Artigo(
    id_pessoa INT NOT NULL,
    id_artigo INT NOT NULL,
    PRIMARY KEY (id_pessoa, id_artigo),
    CONSTRAINT Fk_autor_artigo_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id),
    CONSTRAINT Fk_autor_artigo_artigo FOREIGN KEY (id_artigo) REFERENCES Artigo(id)
);
GO

CREATE TABLE Inscricao_Minicurso(
    id_pessoa INT NOT NULL,
    id_minicurso INT NOT NULL,
    data_inscricao DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (id_pessoa, id_minicurso),
    CONSTRAINT Fk_inscricao_minicurso_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id),
    CONSTRAINT Fk_inscricao_minicurso_minicurso FOREIGN KEY (id_minicurso) REFERENCES Minicurso(id)
);
GO

CREATE TABLE Inscricao_Palestra(
    id_pessoa INT NOT NULL,
    id_palestra INT NOT NULL,
    PRIMARY KEY (id_pessoa, id_palestra),
    CONSTRAINT Fk_inscricao_palestra_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id),
    CONSTRAINT Fk_inscricao_palestra_palestra FOREIGN KEY (id_palestra) REFERENCES Palestra(id)
);
GO

CREATE TABLE Inscricao_Artigo (
    id_pessoa INT NOT NULL,
    id_artigo INT NOT NULL,
    data_inscricao DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (id_pessoa, id_artigo),
    CONSTRAINT Fk_ia_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id),
    CONSTRAINT Fk_ia_artigo FOREIGN KEY (id_artigo) REFERENCES Artigo(id)
);
GO

CREATE TABLE Orador_Minicurso (
    id_minicurso INT NOT NULL,
    id_pessoa INT NOT NULL,
    PRIMARY KEY (id_minicurso, id_pessoa),
    CONSTRAINT Fk_orador_minicurso FOREIGN KEY (id_minicurso) REFERENCES Minicurso(id),
    CONSTRAINT Fk_om_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id)
);
GO

CREATE TABLE Orador_Palestra (
    id_palestra INT NOT NULL,
    id_pessoa INT NOT NULL,
    PRIMARY KEY (id_palestra, id_pessoa),
    CONSTRAINT Fk_orador_palestra FOREIGN KEY (id_palestra) REFERENCES Palestra(id),
    CONSTRAINT Fk_op_pessoa FOREIGN KEY (id_pessoa) REFERENCES Pessoa(id)
);
GO

-- Índices para melhorar performance
CREATE INDEX idx_pessoa_tipo ON Pessoa(tipo_pessoa);
CREATE INDEX idx_artigo_tema ON Artigo(id_tema);
CREATE INDEX idx_evento_data ON Simposio(data_inicio);
CREATE INDEX idx_minicurso_data ON Minicurso(data_evento);
CREATE INDEX idx_palestra_data ON Palestra(data_evento);
GO

-- Triggers
CREATE TRIGGER tr_impedir_inscricao_minicurso 
ON Inscricao_Minicurso
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @id_pessoa INT, @id_minicurso INT, @orador_minicurso INT
    
    SELECT @id_pessoa = id_pessoa, @id_minicurso = id_minicurso FROM inserted
    
    SELECT @orador_minicurso = id_pessoa FROM Orador_Minicurso WHERE id_minicurso = @id_minicurso
    
    IF @id_pessoa = @orador_minicurso
    BEGIN
        RAISERROR('Oradores não podem se inscrever em seus próprios minicursos', 16, 1)
        RETURN
    END
    
    INSERT INTO Inscricao_Minicurso (id_pessoa, id_minicurso, data_inscricao)
    SELECT id_pessoa, id_minicurso, ISNULL(data_inscricao, GETDATE()) FROM inserted
END;
GO

CREATE TRIGGER tr_impedir_inscricao_palestrante 
ON Inscricao_Palestra
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @id_pessoa INT, @id_palestra INT, @orador_palestra INT
    
    SELECT @id_pessoa = id_pessoa, @id_palestra = id_palestra FROM inserted
    
    SELECT @orador_palestra = id_pessoa FROM Orador_Palestra WHERE id_palestra = @id_palestra
    
    IF @id_pessoa = @orador_palestra
    BEGIN
        RAISERROR('Palestrantes não podem se inscrever em suas próprias palestras', 16, 1)
        RETURN
    END
    
    INSERT INTO Inscricao_Palestra (id_pessoa, id_palestra)
    SELECT id_pessoa, id_palestra FROM inserted
END;
GO

CREATE TRIGGER trg_validar_artigo
ON Artigo
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @tema_comissao INT, @comissao_existe INT, @msg_erro VARCHAR(1000)
    
    -- Validação da comissão e tema
    SELECT @comissao_existe = COUNT(*) FROM Comissao C 
    JOIN inserted i ON C.id = i.id_comissao
    
    IF @comissao_existe = 0
    BEGIN
        RAISERROR('Erro: Comissão especificada não existe', 16, 1)
        RETURN
    END
    
    -- Verificar se o tema do artigo corresponde ao tema da comissão
    SELECT @tema_comissao = C.id_tema 
    FROM Comissao C 
    JOIN inserted i ON C.id = i.id_comissao
    
    IF EXISTS (SELECT 1 FROM inserted WHERE id_tema IS NULL OR id_tema != @tema_comissao)
    BEGIN
        SET @msg_erro = 'Erro: O tema do artigo deve ser igual ao tema da comissão (' + 
                        CAST(@tema_comissao AS VARCHAR) + ')'
        RAISERROR(@msg_erro, 16, 1)
        RETURN
    END
    
    -- Inserir os dados com avaliação baseada na nota
    INSERT INTO Artigo (avaliacao, titulo, data_submissao, data_avaliacao, nota_avaliacao, id_comissao, id_tema)
    SELECT 
        CASE 
            WHEN nota_avaliacao IS NULL THEN 'Pendente'
            WHEN nota_avaliacao >= 7 THEN 'Aprovado'
            ELSE 'Reprovado'
        END,
        titulo, data_submissao, data_avaliacao, nota_avaliacao, id_comissao, id_tema
    FROM inserted
END;
GO

-- Inserções na tabela Pessoa
INSERT INTO Pessoa (cpf, nome_completo, email, telefone, sexo, data_nascimento, tipo_pessoa, eh_universitario, cod_universitario) VALUES
('12345678901', 'João Silva', 'joao.silva@email.com', '11987654321', 'M', '1990-05-15', 'Participante', 1, 1001),
('23456789012', 'Maria Oliveira', 'maria.oliveira@email.com', '21987654322', 'F', '1985-08-20', 'Palestrante', 1, 1002),
('34567890123', 'Carlos Pereira', 'carlos.pereira@email.com', '31987654323', 'M', '1992-03-10', 'Organizador', 1, 1003),
('45678901234', 'Ana Santos', 'ana.santos@email.com', '41987654324', 'F', '1988-11-25', 'Participante', 0, NULL),
('56789012345', 'Pedro Costa', 'pedro.costa@email.com', '51987654325', 'M', '1995-07-30', 'Palestrante', 1, 1005),
('67890123456', 'Juliana Almeida', 'juliana.almeida@email.com', '61987654326', 'F', '1991-09-12', 'Participante', 1, 1006),
('78901234567', 'Marcos Souza', 'marcos.souza@email.com', '71987654327', 'M', '1987-04-05', 'Organizador', 0, NULL),
('89012345678', 'Fernanda Lima', 'fernanda.lima@email.com', '81987654328', 'F', '1993-12-18', 'Palestrante', 1, 1008),
('90123456789', 'Ricardo Martins', 'ricardo.martins@email.com', '91987654329', 'M', '1989-06-22', 'Participante', 0, NULL),
('01234567890', 'Patrícia Rocha', 'patricia.rocha@email.com', '11987654330', 'F', '1994-02-28', 'Organizador', 1, 1010),
('11223344556', 'Luiz Fernandes', 'luiz.fernandes@email.com', '11987654331', 'M', '1991-04-18', 'Participante', 1, 1011),
('22334455667', 'Beatriz Castro', 'beatriz.castro@email.com', '21987654332', 'F', '1986-07-23', 'Palestrante', 1, 1012),
('33445566778', 'Gustavo Henrique', 'gustavo.henrique@email.com', '31987654333', 'M', '1993-01-11', 'Organizador', 1, 1013),
('44556677889', 'Daniela Menezes', 'daniela.menezes@email.com', '41987654334', 'F', '1989-10-26', 'Participante', 0, NULL),
('55667788990', 'Roberto Alves', 'roberto.alves@email.com', '51987654335', 'M', '1996-06-01', 'Palestrante', 1, 1015),
('66778899001', 'Camila Ribeiro', 'camila.ribeiro@email.com', '61987654336', 'F', '1992-08-13', 'Participante', 1, 1016),
('77889900112', 'Eduardo Nunes', 'eduardo.nunes@email.com', '71987654337', 'M', '1988-03-06', 'Organizador', 0, NULL),
('88990011223', 'Tatiana Soares', 'tatiana.soares@email.com', '81987654338', 'F', '1994-11-19', 'Palestrante', 1, 1018),
('99001112234', 'Leonardo Campos', 'leonardo.campos@email.com', '91987654339', 'M', '1990-05-23', 'Participante', 0, NULL),
('00112233445', 'Vanessa Dias', 'vanessa.dias@email.com', '11987654340', 'F', '1995-01-29', 'Organizador', 1, 1020);
GO

-- Inserções na tabela tema
INSERT INTO Tema (nome, descricao, area_conhecimento, data_criacao) VALUES
('Inteligência Artificial', 'Técnicas avançadas de IA e machine learning', 'Ciência da Computação', GETDATE()),
('Biologia Molecular', 'Pesquisas em genética e biologia celular', 'Biologia', GETDATE()),
('Economia Sustentável', 'Modelos econômicos para desenvolvimento sustentável', 'Economia', GETDATE()),
('Educação Inclusiva', 'Métodos pedagógicos para inclusão social', 'Educação', GETDATE()),
('Energias Renováveis', 'Tecnologias para geração de energia limpa', 'Engenharia', GETDATE()),
('Blockchain', 'Tecnologias de ledger distribuído e criptomoedas', 'Ciência da Computação', GETDATE()),
('Ecologia Marinha', 'Estudos sobre ecossistemas oceânicos', 'Biologia', GETDATE()),
('Mercado de Capitais', 'Análise de investimentos e bolsas de valores', 'Economia', GETDATE()),
('Educação Digital', 'Tecnologias aplicadas ao ensino remoto', 'Educação', GETDATE()),
('Energia Nuclear', 'Tecnologias para geração de energia nuclear segura', 'Engenharia', GETDATE());
GO

-- Inserções na tabela Comissao
INSERT INTO Comissao (nome, id_tema, descricao, data_criacao) VALUES
('Comissão de Tecnologia', 1, 'Avalia trabalhos na área de tecnologia', GETDATE()),
('Comissão de Biociências', 2, 'Avalia trabalhos na área de biologia', GETDATE()),
('Comissão de Humanidades', 4, 'Avalia trabalhos na área de educação', GETDATE()),
('Comissão de Engenharia', 5, 'Avalia trabalhos na área de engenharia', GETDATE()),
('Comissão de Economia', 3, 'Avalia trabalhos na área econômica', GETDATE()),
('Comissão de Inovação', 6, 'Avalia trabalhos na área de tecnologias disruptivas', GETDATE()),
('Comissão de Ciências do Mar', 7, 'Avalia trabalhos na área de oceanografia', GETDATE()),
('Comissão de Tecnologia Educacional', 9, 'Avalia trabalhos na área de educação digital', GETDATE()),
('Comissão de Energia', 10, 'Avalia trabalhos na área de energia nuclear', GETDATE()),
('Comissão de Finanças', 8, 'Avalia trabalhos na área de mercado financeiro', GETDATE());
GO

-- Inserções na tabela Simposio
INSERT INTO Simposio (titulo, descricao, universidade, data_inicio, data_fim, id_orador_principal, local_simposio, capacidade) VALUES
('Simpósio de Tecnologia 2023', 'Evento sobre as últimas tendências em TI', 'Universidade Federal de Tecnologia', '2023-11-15', '2023-11-18', 2, 'Centro de Convenções da Cidade', 500),
('Simpósio de Ciências Biológicas', 'Discussões sobre avanços em biologia', 'Universidade de Biociências', '2023-10-20', '2023-10-22', 5, 'Campus Universitário - Bloco B', 300),
('Fórum de Educação Moderna', 'Debates sobre metodologias de ensino', 'Faculdade de Educação', '2023-09-05', '2023-09-07', 8, 'Auditório Principal', 200),
('Simpósio de Inovação 2023', 'Evento sobre tecnologias disruptivas', 'Universidade Federal de Inovação', '2023-12-10', '2023-12-13', 12, 'Centro de Eventos Tecnológicos', 600),
('Simpósio de Ciências do Mar', 'Discussões sobre oceanos e vida marinha', 'Universidade de Oceanografia', '2023-11-25', '2023-11-27', 15, 'Campus Costeiro - Auditório Azul', 350),
('Fórum de Educação Digital', 'Debates sobre o futuro do ensino online', 'Faculdade de Tecnologia Educacional', '2023-10-15', '2023-10-17', 18, 'Auditório Virtual', 250);
GO

-- Inserções na tabela Minicurso
INSERT INTO Minicurso (titulo, data_evento, hora_inicio, hora_fim, id_tema) VALUES
('Introdução ao Deep Learning', '2023-11-16', '09:00:00', '12:00:00', 1),
('Técnicas de PCR Avançada', '2023-10-21', '14:00:00', '17:00:00', 2),
('Metodologias Ativas de Aprendizagem', '2023-09-06', '10:00:00', '13:00:00', 4),
('Introdução ao Blockchain', '2023-12-11', '10:00:00', '12:00:00', 6),
('Técnicas de Monitoramento Marinho', '2023-11-26', '15:00:00', '18:00:00', 7),
('Plataformas de EAD', '2023-10-16', '11:00:00', '14:00:00', 9);
GO

-- Inserções na tabela Palestra
INSERT INTO Palestra (titulo, data_evento, hora_inicio, hora_fim, id_tema) VALUES
('O Futuro da IA Generativa', '2023-11-17', '14:00:00', '16:00:00', 1),
('Descobertas em Genômica', '2023-10-22', '09:00:00', '11:00:00', 2),
('Educação Pós-Pandemia', '2023-09-07', '16:00:00', '18:00:00', 4),
('O Futuro das Criptomoedas', '2023-12-12', '15:00:00', '17:00:00', 6),
('Descobertas em Recifes de Coral', '2023-11-27', '10:00:00', '12:00:00', 7),
('Gamificação na Educação', '2023-10-17', '17:00:00', '19:00:00', 9);
GO

-- Inserções na tabela Artigo
INSERT INTO Artigo (avaliacao, titulo, data_submissao, data_avaliacao, nota_avaliacao, id_comissao, id_tema) VALUES
('Aprovado', 'Redes Neurais para Processamento de Linguagem Natural', '2023-08-10', '2023-09-01', 8, 1, 1),
('Reprovado', 'Análise de Mutação Genética em Drosophila', '2023-07-15', '2023-08-01', 5, 2, 2),
('Aprovado', 'Modelos de Aprendizagem Baseada em Projetos', '2023-06-20', '2023-07-10', 9, 3, 4),
('Aprovado', 'Contratos Inteligentes em Ethereum', '2023-09-15', '2023-10-01', 9, 6, 6),
('Reprovado', 'Impacto do Aquecimento Global nos Oceanos', '2023-08-20', '2023-09-05', 4, 7, 7),
('Aprovado', 'Eficácia das Plataformas de Videoconferência', '2023-07-25', '2023-08-15', 8, 9, 10);
GO

-- Inserções nas tabelas de relacionamento
INSERT INTO Comissao_Pessoa (id_pessoa, id_comissao) VALUES
(3, 1),
(7, 2),
(10, 3),
(13, 6),
(17, 7),
(20, 9);
GO

INSERT INTO Inscrito_Simposio (id_evento, id_pessoa, data_inscricao) VALUES
(1, 1, GETDATE()),
(1, 4, GETDATE()),
(2, 6, GETDATE()),
(3, 9, GETDATE()),
(4, 11, GETDATE()),
(4, 14, GETDATE()),
(5, 16, GETDATE()),
(6, 19, GETDATE());
GO

INSERT INTO Minicurso_Simposio (id_simposio, id_minicurso) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 6);
GO

INSERT INTO Artigo_Simposio (id_simposio, id_artigo) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 6);
GO

INSERT INTO Palestra_Simposio (id_simposio, id_palestra) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 6);
GO

INSERT INTO Organizador_Simposio (id_pessoa, id_simposio) VALUES
(3, 1),
(7, 2),
(10, 3),
(13, 4),
(17, 5),
(20, 6);
GO

INSERT INTO Autor_Artigo (id_pessoa, id_artigo) VALUES
(1, 1),
(6, 2),
(9, 3),
(11, 4),
(16, 5),
(19, 6);
GO

INSERT INTO Inscricao_Minicurso (id_pessoa, id_minicurso, data_inscricao) VALUES
(1, 1, GETDATE()),
(4, 1, GETDATE()),
(6, 2, GETDATE()),
(9, 3, GETDATE()),
(11, 4, GETDATE()),
(14, 4, GETDATE()),
(16, 5, GETDATE()),
(19, 6, GETDATE());
GO

INSERT INTO Inscricao_Palestra (id_pessoa, id_palestra) VALUES
(1, 1),
(4, 1),
(6, 2),
(9, 3),
(11, 4),
(14, 4),
(16, 5),
(19, 6);
GO

INSERT INTO Orador_Minicurso (id_minicurso, id_pessoa) VALUES
(1, 2),
(2, 5),
(3, 8),
(4, 12),
(5, 15),
(6, 18);
GO

INSERT INTO Orador_Palestra (id_palestra, id_pessoa) VALUES
(1, 2),
(2, 5),
(3, 8),
(4, 12),
(5, 15),
(6, 18);
GO

-- 1. Consulta básica de todas as tabelas principais
SELECT 'Pessoa' AS Tabela, COUNT(*) AS Registros FROM Pessoa
UNION ALL
SELECT 'Tema', COUNT(*) FROM Tema
UNION ALL
SELECT 'Comissao', COUNT(*) FROM Comissao
UNION ALL
SELECT 'Minicurso', COUNT(*) FROM Minicurso
UNION ALL
SELECT 'Artigo', COUNT(*) FROM Artigo
UNION ALL
SELECT 'Palestra', COUNT(*) FROM Palestra
UNION ALL
SELECT 'Simposio', COUNT(*) FROM Simposio;
GO

-- 2. Pessoas universitárias com código universitário
SELECT 
    nome_completo AS nome_pessoa,
    cod_universitario AS codigo_universitario
FROM 
    Pessoa
WHERE 
    eh_universitario = 1
ORDER BY 
    nome_completo ASC;
GO

-- 3. Minicursos do simpósio 1
SELECT 
    M.id AS id_minicurso,
    M.titulo AS titulo_minicurso
FROM 
    Simposio AS S
JOIN 
    Minicurso_Simposio AS MS ON S.id = MS.id_simposio
JOIN 
    Minicurso AS M ON MS.id_minicurso = M.id
WHERE 
    S.id = 1;
GO

-- 4. Minicursos de todos os simpósios (versão alternativa para SQL Server)
SELECT 
    S.id AS id_simposio,
    S.titulo AS titulo_simposio,
    STUFF((
        SELECT ' - ' + M.titulo 
        FROM Minicurso M
        JOIN Minicurso_Simposio MS ON M.id = MS.id_minicurso
        WHERE MS.id_simposio = S.id
        FOR XML PATH('')
    ), 1, 3, '') AS minicursos
FROM 
    Simposio AS S
ORDER BY 
    S.id;
GO

-- 5. Participantes e palestras inscritas
SELECT
    P.id AS id_participante,
    P.nome_completo AS nome_participante,
    PAL.titulo AS titulo_palestra,
    S.titulo AS titulo_simposio
FROM
    Pessoa AS P
JOIN
    Inscricao_Palestra AS IP ON P.id = IP.id_pessoa
JOIN
    Palestra AS PAL ON IP.id_palestra = PAL.id
JOIN
    Palestra_Simposio AS PS ON PAL.id = PS.id_palestra
JOIN
    Simposio AS S ON PS.id_simposio = S.id
WHERE
    P.tipo_pessoa = 'Participante'
ORDER BY
    P.nome_completo, S.titulo;
GO

-- 6. Quantidade de participantes por simpósio
SELECT 
    S.titulo AS titulo_simposio,
    COUNT(IS_.id_pessoa) AS total_inscritos
FROM 
    Simposio AS S
LEFT JOIN 
    Inscrito_Simposio AS IS_ ON S.id = IS_.id_evento  
GROUP BY 
    S.id, S.titulo;
GO

-- 7. Quantidade de universitários por simpósio
SELECT 
    S.titulo AS titulo_simposio,
    SUM(CASE WHEN P.eh_universitario = 1 THEN 1 ELSE 0 END) AS total_universitarios
FROM 
    Simposio AS S
LEFT JOIN 
    Inscrito_Simposio AS IS_ ON S.id = IS_.id_evento
LEFT JOIN 
    Pessoa AS P ON IS_.id_pessoa = P.id
GROUP BY 
    S.id, S.titulo;
GO

-- 8. Lista de palestrantes
SELECT 
    P.id AS id_palestrante,
    P.nome_completo AS nome_palestrante,
    P.cpf,
    P.email AS email_palestrante
FROM 
    Pessoa AS P
WHERE 
    P.tipo_pessoa = 'Palestrante';
GO

-- 9. Organizadores dos simpósios (versão SQL Server)
SELECT 
    S.titulo AS titulo_simposio,
    STRING_AGG(P.nome_completo, ', ') WITHIN GROUP (ORDER BY P.nome_completo ASC) AS organizadores
FROM 
    Simposio AS S
LEFT JOIN 
    Organizador_Simposio AS OS ON S.id = OS.id_simposio
LEFT JOIN 
    Pessoa AS P ON OS.id_pessoa = P.id
GROUP BY 
    S.id, S.titulo
ORDER BY 
    S.titulo ASC;
GO

-- 10. Palestras com dados do orador
SELECT 
    PAL.titulo AS titulo_palestra,
    FORMAT(PAL.data_evento, 'dd/MM/yyyy') AS data_palestra,
    FORMAT(PAL.hora_inicio, 'HH:mm') AS hora_inicio,
    FORMAT(PAL.hora_fim, 'HH:mm') AS hora_fim,
    P.nome_completo AS nome_orador,
    P.email AS email_orador,
    P.cpf AS cpf_orador
FROM 
    Palestra AS PAL
JOIN 
    Orador_Palestra AS OP ON PAL.id = OP.id_palestra
JOIN 
    Pessoa AS P ON OP.id_pessoa = P.id
WHERE
    P.tipo_pessoa = 'Palestrante'
ORDER BY 
    PAL.data_evento ASC, 
    PAL.hora_inicio ASC;
GO

-- 11. Universitários que também são palestrantes
SELECT 
    P.nome_completo AS nome_pessoa,
    P.cpf,
    P.cod_universitario,
    P.email,
    P.telefone
FROM 
    Pessoa AS P
WHERE 
    P.tipo_pessoa = 'Palestrante'
    AND P.eh_universitario = 1
ORDER BY 
    P.nome_completo ASC;
GO

-- 12. Minicursos com maiores números de inscritos
SELECT 
    M.titulo AS minicurso,
    COUNT(IM.id_pessoa) AS total_inscritos
FROM Minicurso AS M
LEFT JOIN Inscricao_Minicurso AS IM ON M.id = IM.id_minicurso
GROUP BY M.id, M.titulo
ORDER BY total_inscritos DESC;
GO

-- 13. Horários das palestras e palestrantes (versão SQL Server)
SELECT 
    P.nome_completo AS palestrante,
    COUNT(OP.id_palestra) AS total_palestras,
    STRING_AGG(
        CONCAT(
            PAL.titulo, ' (', FORMAT(PAL.data_evento, 'dd/MM/yyyy'), ' - ', 
            FORMAT(PAL.hora_inicio, 'HH:mm'), ' às ', FORMAT(PAL.hora_fim, 'HH:mm'), ')'
        ), 
        '; '
    ) WITHIN GROUP (ORDER BY PAL.data_evento, PAL.hora_inicio) AS detalhes_palestras
FROM 
    Pessoa AS P
JOIN 
    Orador_Palestra AS OP ON P.id = OP.id_pessoa
JOIN 
    Palestra AS PAL ON OP.id_palestra = PAL.id
GROUP BY 
    P.id, P.nome_completo;
GO

-- 14. Porcentagem de ocupação simpósio
SELECT 
    S.titulo AS simposio,
    S.capacidade,
    COUNT(IS_.id_pessoa) AS inscritos,
    CASE 
        WHEN S.capacidade = 0 THEN '0%'
        ELSE CONCAT(
            ROUND(
                (COUNT(IS_.id_pessoa) * 100.0 / S.capacidade), 1
            ), 
            '%'
        )
    END AS ocupacao
FROM Simposio AS S
LEFT JOIN Inscrito_Simposio AS IS_ ON S.id = IS_.id_evento
GROUP BY S.id, S.titulo, S.capacidade;
GO

-- 15. Inscrições feitas depois de 26/04/2025
SELECT 
    S.titulo AS simposio,
    P.nome_completo AS participante,
    FORMAT(IS_.data_inscricao, 'dd/MM/yyyy HH:mm') AS data_inscricao_br
FROM Inscrito_Simposio AS IS_
JOIN Simposio AS S ON IS_.id_evento = S.id
JOIN Pessoa AS P ON IS_.id_pessoa = P.id
WHERE IS_.data_inscricao >= '2025-04-26';
GO

-- 16. Quantidade de artigos aprovados e reprovados por comissão
SELECT 
    C.nome AS comissao,
    SUM(CASE WHEN A.avaliacao = 'Aprovado' THEN 1 ELSE 0 END) AS aprovados,
    SUM(CASE WHEN A.avaliacao = 'Reprovado' THEN 1 ELSE 0 END) AS reprovados,
    ROUND(AVG(CAST(A.nota_avaliacao AS FLOAT)), 1) AS media_notas
FROM Comissao AS C
LEFT JOIN Artigo AS A ON C.id = A.id_comissao
GROUP BY C.id, C.nome;
GO

-- 17. Relação de artigos por tema
SELECT 
    T.nome AS tema,
    COUNT(A.id) AS total_artigos,
    SUM(CASE WHEN A.avaliacao = 'Aprovado' THEN 1 ELSE 0 END) AS aprovados,
    SUM(CASE WHEN A.avaliacao = 'Reprovado' THEN 1 ELSE 0 END) AS reprovados,
    SUM(CASE WHEN A.avaliacao = 'Pendente' THEN 1 ELSE 0 END) AS pendentes
FROM Tema AS T
LEFT JOIN Artigo AS A ON T.id = A.id_tema
GROUP BY T.id, T.nome
ORDER BY total_artigos DESC;
GO

-- 18. Eventos por data (palestras e minicursos)
SELECT 
    'Palestra' AS tipo_evento,
    titulo,
    data_evento,
    hora_inicio,
    hora_fim
FROM Palestra
UNION ALL
SELECT 
    'Minicurso' AS tipo_evento,
    titulo,
    data_evento,
    hora_inicio,
    hora_fim
FROM Minicurso
ORDER BY data_evento, hora_inicio;
GO