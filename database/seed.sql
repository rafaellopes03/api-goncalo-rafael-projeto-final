USE [QuintaAzenhaDB]
GO

-- Vinhos
SET IDENTITY_INSERT [dbo].[Vinhos] ON
GO

INSERT INTO [dbo].[Vinhos] ([Id], [Nome], [Tipo], [Ano], [Descricao], [Preco], [Imagem], [Disponivel], [Sku])
VALUES
(1, 'Arinto Clássico', 'branco', 2022, 'Fresco e mineral, com notas cítricas típicas de Bucelas.', 12.50, 'arinto_classico.png', 1, 'QA-001'),
(2, 'Arinto Reserva', 'reserva', 2020, 'Maior complexidade e estágio em madeira. Elegante e persistente.', 18.00, 'arinto_reserva.png', 1, 'QA-002'),
(3, 'Arinto Colheita Tardia', 'colheita', 2021, 'Notas de mel e fruta madura. Acidez vibrante em equilíbrio.', 22.00, 'arinto_colheita.png', 1, 'QA-003')
GO

SET IDENTITY_INSERT [dbo].[Vinhos] OFF
GO

-- Experiências
SET IDENTITY_INSERT [dbo].[Experiencias] ON
GO

INSERT INTO [dbo].[Experiencias] ([Id], [Nome], [Descricao], [Preco], [DuracaoMinutos], [MaxPessoas], [Imagem], [Disponivel])
VALUES
(1, 'Prova de Arinto', 'Uma viagem pelos solos de Bucelas através dos nossos melhores Arintos.', 25.00, 90, 20, 'prova_vinhos.png', 1),
(2, 'Visita às Vinhas', 'Passeio guiado pelas vinhas centenárias de Arinto.', 15.00, 60, 15, 'QtaAzenha.png', 1),
(3, 'Workshop Vindima', 'Participe na colheita manual das uvas Arinto.', 40.00, 180, 12, 'prova_vinhos.png', 1),
(4, 'Jantar na Adega', 'Menu tradicional português harmonizado com os nossos vinhos.', 60.00, 150, 10, 'jantar.png', 1),
(5, 'Jacuzzi', 'Bolhas e Vinho', 80.00, 60, 4, 'jacuzzi.jpg', 1)
GO

SET IDENTITY_INSERT [dbo].[Experiencias] OFF
GO