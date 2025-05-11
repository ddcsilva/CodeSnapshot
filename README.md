# CodeSnapshot

CodeSnapshot é uma aplicação desktop desenvolvida em .NET MAUI que permite gerar snapshots completos de projetos de código-fonte, facilitando a documentação e o compartilhamento de código.

## 🚀 Funcionalidades

- Exportação de projetos .NET e Angular
- Seleção personalizada da pasta de destino
- Interface moderna e responsiva
- Suporte a tema claro e escuro
- Logs detalhados da exportação
- Cópia de caminhos para área de transferência
- Visualização do progresso em tempo real

## 📋 Pré-requisitos

- Windows 10 versão 19041.0 ou superior
- .NET 8.0 SDK
- Visual Studio 2022 (recomendado)

## 🔧 Instalação

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/CodeSnapshot.git
```

2. Abra a solução no Visual Studio 2022

3. Restaure os pacotes NuGet:
```bash
dotnet restore
```

4. Compile o projeto:
```bash
dotnet build
```

## 🎮 Como Usar

1. Selecione o tipo de projeto (.NET ou Angular)
2. Escolha a pasta do projeto que deseja exportar
3. (Opcional) Selecione uma pasta de destino personalizada
4. Clique em "Generate Snapshot"
5. Aguarde a conclusão da exportação
6. Visualize o arquivo gerado ou salve o log

## 📁 Estrutura do Projeto

```
CodeSnapshot/
├── Resources/
│   ├── Images/         # Ícones e imagens
│   ├── Localization/   # Arquivos de localização
│   └── Fonts/          # Fontes personalizadas
├── Services/
│   ├── IProjectExporter.cs
│   ├── DotnetProjectExporter.cs
│   └── LogService.cs
└── MainPage.xaml       # Interface principal
```

## 🛠️ Tecnologias Utilizadas

- .NET MAUI
- C# 12
- XAML
- Windows App SDK

## 👨‍💻 Desenvolvido por

**Danilo Silva**

- GitHub: [@seu-usuario](https://github.com/seu-usuario)
- LinkedIn: [Danilo Silva](https://linkedin.com/in/seu-perfil)

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🤝 Contribuindo

1. Faça o fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📫 Suporte

Para suporte, envie um email para seu-email@exemplo.com ou abra uma issue no GitHub.

## 🙏 Agradecimentos

- [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [GitHub](https://github.com)

---
⭐️ From [Danilo Silva](https://github.com/seu-usuario)