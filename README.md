# README

## 制作背景

## 想定ユーザー
主に以下の様なユーザーを想定しています
- 国内の旅行会社・観光バス会社による事前の駐車場手配
- バスドライバー個人による当日の駐車場予約
- 小学校等の法人の代表者が、部活動で利用するマイクロバスのための駐車場手配
- 一般の家族の旅行のレンタカー駐車

## 開発環境
OS ：Windows11 on WSL2 Ubuntu 24.01
 エディタ： VSCode

### バックエンド技術スタック
言語：C# .NET 10
フレームワーク：ASP.NET Core / FastEndpoints
データベース：PostgresSQL 18
セッション管理：Redis 8
データベースライブラリ： Dapper / Npgsql.EntityFrameworkCore.PostgreSQL
テストフレームワーク：XUnit v3 / Shoudly / TestContainers / FastEndpoints.Testing
認証：ASP.NET Core Authentication / Cookie Authentication / FastEndpoint.Security

## データベース設計
ER図
### テーブル一覧

## 機能一覧

## 実装機能一覧
### 管理者用API
- [ ] 駐車場の再有効化API（管理者）

- [ ] 管理者向けの一覧・検索
  - [ ] 全予約の検索（駐車場・期間・ステータスで絞り込み）
  - [ ] アカウント一覧
  - [ ] 駐車場一覧（無効なものも含む）
- [ ] アカウント停止（管理者。`Suspend()` は定義済みで未使用）
- [ ] 出庫時刻を過ぎた予約を `Completed` にするバッチ
- [ ] LotAdmin（駐車場管理者）ロールの権限設計と実装
- [ ] サインイン時に `LastLoginAt` を更新する
- [ ] サインイン時に停止中アカウントを弾く処理を確認する

### ユーザー用
- [ ] 駐車場の一覧・検索API（利用者向け）
- [ ] 空き状況（残り台数）の照会API
- [ ] メール認証の実装
- [ ] 管理者用の予約削除を実装する（今の `DeleteReservation` は中身がない）

- [ ] アカウント管理
  - [ ] プロフィール更新（利用者）
  - [ ] パスワード変更・リセット（利用者）
  - [ ] 退会（利用者）

- [ ] 一覧APIのページング
- [ ] ヘルスチェック（PostgreSQL と Redis）
- [ ] 構造化ログ・監査ログ（予約の変更やキャンセルを誰がいつ行ったか）
- [ ] サインイン・サインアップのレートリミット
- [ ] CORS 設定
- [ ] 予約の確定・変更・キャンセル時のメール通知
- [ ] `campanies` テーブルの扱いを決める（`UserType.Business` と結び付けるか、削除するか）

...
