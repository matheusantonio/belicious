# Belicious - An URL Bookmark Application

*Aplicação em desenvolvimento para a disciplina de Programação para Internet.*

A aplicação consiste em uma ferramenta para salvar URLs de sites que o usuário ache interessante. Entre suas funcionalidades, destacam-se:

 - Bookmarks armazenadas por usuário, com a possibilidade de inserção, edição, remoção e visualização de bookmarks
 - Página inicial exibindo as URLs mais adicionadas pelos usuários e as URLs que foram adicionadas recentemente
 - Possibilidade de criação de bookmarks privadas (não são exibidas na página inicial)
  
A aplicação foi feita com o intuito de aprendizado, principalmente do framework ASP .NET Core, mas também de conceitos de desenvolvimento backend como:

- Autenticação e Autorização, utilizando as ferramentas do ASP.NET core
- Web Scrapping, analisando o conteúdo HTML da URL adicionada para armazenar um nome comum a todos os usuários (exibido na página inicial) através do title ou h1 da página
- Persistência de Dados através do Entity Framework Core usando o SQLite
- Arquitetura MVC, separando as responsabilidades de controladores, modelos e views
- Rotas e métodos HTTP para navegação pela aplicação

