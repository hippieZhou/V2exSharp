<div align="center">

# V2exSharp

![.Net](https://img.shields.io/badge/.NET-5C2D91?style=flat&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=flat&logo=c-sharp&logoColor=white)
![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/hippieZhou/V2exSharp/.NET/main)
![GitHub license](https://img.shields.io/github/license/hippieZhou/V2exSharp)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/hippieZhou/V2exSharp)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/hippieZhou/V2exSharp/main)

</div>

## Features

- [x] Use `FluentAPI` to integrate
- [x] Use `Async` way to post

## API Scope

### V1

| 接口   | 路径                   | 进度  |
|------|-------------------------|-----|
| 最热主题 | /api/topics/hot.json    | &#9745; |
| 最新主题 | /api/topics/latest.json | &#9745; |
| 节点信息 | /api/nodes/show.json    | &#9745; |
| 用户主页 | /api/members/show.json  | &#9745; |

### V2

| 接口            | 路径                           | 进度 |
|----------------------|--------------------------------|---------|
| 获取最新的提醒       | notifications                  | &#9745; |
| 删除指定的提醒       | notifications/:notification_id | &#9745; |
| 获取自己的 Profile   | member                         | &#9745; |
| 查看当前使用的令牌   | token                          | &#9745; |
| 获取指定节点         | nodes/:node_name               | &#9745; |
| 获取指定节点下的主题 | nodes/:node_name/topics        | &#9745; |
| 获取指定主题         | topics/:topic_id               | &#9745; |
| 获取指定主题下的回复 | topics/:topic_id/replies       | &#9745; |

### Install

```bash
Install-Package V2exSharp
# or
dotnet add package V2exSharp
```

## Generate Nuget Package

```bash
dotnet build --configuration Release
```

## Reference

- [API](https://v2ex.com/help/api)
- [V2exOS](https://github.com/isaced/V2exOS)
- [V2exAPI](https://github.com/isaced/V2exAPI)