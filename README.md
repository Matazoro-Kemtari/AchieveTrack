# 受注管理実績登録

AchieveTrack

- [1. 使用方法](#1-使用方法)
- [2. エラーメッセージ内容](#2-エラーメッセージ内容)
- [3. 技術情報](#3-技術情報)
  - [3.1. データクラスの suffix](#31-データクラスの-suffix)

## 1. 使用方法

1. アプリケーションを起動します。

   <img src="images/MainWindow.png" width="60%" />

2. 受注管理日報エクセルファイルをドラッグ＆ドロップします。
3. 日報の内容が表示されます。

   <img src="images/DroppedWindow.png" width="60%" />

   1. エラーが表示されている場合は、エラー内容を解消してください。

4. 「登録」ボタンをクリックします。

   - 設計部専用の機能として、登録の際に「設計情報に追加する」にチェックを入れると、設計情報に追加されます。
     その際、設計管理未登録のエラーは無視されます。

5. 登録完了のメッセージが表示されます。

   <img src="images/FinishingMessage.png" />

## 2. エラーメッセージ内容

| メッセージ                                                  | 内容                                                   | 対処方法                                                                                                                                 |
| ----------------------------------------------------------- | ------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------- |
| この作業日と社員番号の組み合わせが 実績処理で既に存在します | 受注管理に作業日と社員の組み合わせで受注管理に登録済み | このアプリケーションは、受注管理の内容の上書きはしていません。必要に応じて、受注管理の機能で、既存の実績を削除してから登録してください。 |
| 作業台帳にない作業番号です                                  | 作業番号が受注管理に登録されていない                   | 工数転送システムで、作業番号を間違えたと思われます。確認して再転送してください。                                                         |
| 設計管理に未登録の作業番号です                              | 作業番号が設計情報に登録されていない                   | 作業番号が間違いないことを確認して、設計情報に追加するチェックを入れて登録してください。                                                 |
| 作業日が完成を過ぎています                                  | 作業日が完成日を過ぎている                             | 工数転送システムで、作業番号を間違えたと思われます。確認して再転送してください。                                                         |

## 3. 技術情報

![su図](images/suDiagram.svg)
![do図](images/doDiagram.svg)

### 3.1. データクラスの suffix

```plantuml
@startuml
rectangle 通常 {
    object "プレゼンテーション層" as Presentation
    object "インフラ層" as Infra
    object "ユースケース層" as Application
    object "ドメイン層" as Domain

    Presentation -[hidden] Infra
    Presentation --[hidden] Application
    Application --[hidden] Domain

    Presentation -[#red]-> Application : XxxParam
    Application -[#red]-> Presentation : XxxResult

    Application -[#blue]-> Domain : プリミティブ型/Entity
    Domain .[#blue].> Infra : インターフェイス経由
    Infra .[#blue].> Application : Entity
    Domain -[#blue]-> Application : Entity
}
@enduml
```

```plantuml
@startuml
rectangle クエリサービス {
    object "プレゼンテーション層" as Presentation
    object "インフラ層" as Infra
    object "ユースケース層" as Application
    object "ドメイン層" as Domain

    Presentation -[hidden] Infra
    Presentation --[hidden] Application
    Application --[hidden] Domain

    Application -[#green]-> Infra : XxxAttempt
    Infra -[#green]-> Application : XxxRecord/Entity
}
@enduml
```

```plantuml
@startuml
rectangle ドメインサービス {
    object "プレゼンテーション層" as Presentation
    object "インフラ層" as Infra
    object "ユースケース層" as Application
    object "ドメイン層" as Domain

    Presentation -[hidden] Infra
    Presentation --[hidden] Application
    Application --[hidden] Domain

    Application -[#orange]-> Domain : プリミティブ型/Entity
    Domain -[#orange]-> Application : プリミティブ型/Entity
}
@enduml
```
