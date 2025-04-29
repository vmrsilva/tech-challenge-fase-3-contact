# Tech Challenge FIAP

## Fase 3 - Arquitetura de Sistemas .NET

Nome: Valmir Severino da Silva <br/>
RM: 360650


## Executar projeto: <br/>
### Iniciar container Redis:<br/>
``docker run -d -p:6379:6379 redis:latest``

Acessar o diretório do projeto TechChallange.Contact.Api <br/> 
Executar o comando:
` dotnet run `


### Executar testes: <br/>
Acessar o diretório do projeto TechChallange.Contact.Tests <br/>
Executar o comando:
`dotnet test`

---------------------------------


### Executar a solução completa Microservicos + monitoramento em localhost:<br/>

Clonar o repositório [docker-compose](https://github.com/vmrsilva/tech-challenge-fase3-dockercompose) com o arquivo docker-compose, configurações do Prometheus e Grafana e seguir as orientações para execução.

