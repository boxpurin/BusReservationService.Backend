-- Add migration script here
COMMENT ON SCHEMA "public" IS 'standard public schema';
-- Type / Domain

-- Tables
CREATE TABLE IF NOT EXISTS "public"."accounts" (
   "id" "uuid" PRIMARY KEY default "gen_random_uuid"(), -- アカウントID（データベース用）
   "email" "text" UNIQUE NOT NULL, -- メールアドレス（ログインID）
   "password_hash" "bytea" NOT NULL, -- パスワードハッシュ
   "password_salt" "bytea" NOT NULL, -- パスワードソルト
   "account_status" "text" NOT NULL DEFAULT 'pending_email_verification' CHECK (
       account_status IN (
         'just_created',
       'pending_email_verification', -- メールアドレス確認待ち
       'onboarding', -- オンボーディング中(プロフィール入力待ち)
       'activated', -- 有効化済み
       'suspended' -- 停止中
   )), -- アカウントの状態
   "account_role" "text" NOT NULL CHECK (
       account_role IN (
       'guest', -- ゲスト
       'user',  -- User
       'admin', -- 管理者
       'lot_admin' -- 駐車場管理者
   )), -- アカウントの種類
   "activated_at" timestamp with time zone, -- アカウントの有効化日時
   "last_login_at" timestamp with time zone, -- アカウントの有効化日時
   "created_at" timestamp with time zone DEFAULT "now"() NOT NULL, -- アカウント作成日時
   "updated_at" timestamp with time zone DEFAULT "now"() NOT NULL -- アカウント更新日時
);
COMMENT ON TABLE "public"."accounts" IS 'サービスのアカウント情報';


CREATE TABLE IF NOT EXISTS "public"."users" (
   "account_id" "uuid" PRIMARY KEY,
   CONSTRAINT fk_account FOREIGN KEY ("account_id")
   REFERENCES "public"."accounts"("id")
   ON UPDATE CASCADE
   ON DELETE cascade,
   "first_name" "text" NOT NULL,
   "last_name" "text" NOT NULL,
   "zip_code" varchar(7) NOT NULL,
   "street_address" "text" NOT NULL,
   "phone_number" "text" NOT NULL,
   "fax_number" "text",
   "user_type" "text" NOT NULL CHECK(
      user_type IN (
         'personal',
         'business',
         'administrator'
   )),
   "created_at" timestamp with time zone DEFAULT "now"() NOT NULL,
   "updated_at" timestamp with time zone DEFAULT "now"() NOT null
);
COMMENT ON TABLE "public"."users" IS 'アカウントプロフィール情報';


CREATE TABLE IF NOT EXISTS "public"."campanies" (
   "account_id" "uuid" PRIMARY key,
   CONSTRAINT fk_account FOREIGN KEY ("account_id")
   REFERENCES "public"."accounts"("id")
   ON UPDATE CASCADE
   ON DELETE cascade,
   "name" "text" NOT NULL,
   "zip_code" varchar(7) NOT NULL,
   "street_address" "text" NOT NULL,
   "phone_number" "text" NOT NULL,
   "fax_number" "text",
   "created_at" timestamp with time zone DEFAULT "now"() NOT NULL,
   "updated_at" timestamp with time zone DEFAULT "now"() NOT NULL
);
COMMENT ON TABLE "public"."campanies" IS '企業情報';
CREATE TABLE IF NOT EXISTS "public"."prefectures" (
   "id" smallint primary KEY,
   "name" varchar(10) NOT NULL,
   "kana" varchar(20) NOT NULL
);
COMMENT ON TABLE "public"."prefectures" IS '都道府県情報 (JIS X 0401)';


CREATE TABLE IF NOT EXISTS "public"."parking_lots" (
   "id" "uuid" PRIMARY KEY default "gen_random_uuid"(),
   "display_name" "text" NOT NULL,
   "prefecture_id" smallint NOT null,
   constraint fk_prefecture FOREIGN key ("prefecture_id")
   REFERENCES "public"."prefectures"("id")
   ON UPDATE CASCADE
   ON DELETE NO ACTION,
   "zip_code" varchar(7) NOT NULL,
   "street_address" "text" NOT NULL,
   "phone_number" "text" NOT NULL,
   "fax_number" "text",
   "max_capacity" smallint NOT NULL,
   "reservation_capacity" smallint NOT NULL,
   "is_allows_boarding_alighting" boolean DEFAULT false NOT NULL,
   "is_active" boolean DEFAULT false NOT NULL,
   "created_at" timestamp with time zone DEFAULT "now"() NOT NULL,
   "updated_at" timestamp with time zone DEFAULT "now"() NOT NULL
);
COMMENT ON TABLE "public"."parking_lots" IS '駐車場情報';
CREATE TABLE IF NOT EXISTS "public"."reservations" (
   "id" "uuid" PRIMARY KEY default "gen_random_uuid"(),
  
   "account_id" "uuid" NOT null,
   constraint fk_account FOREIGN KEY ("account_id")
   REFERENCES "public"."accounts"("id")
   ON UPDATE CASCADE
   ON DELETE NO ACTION,
  
   "parking_lot_id" "uuid" NOT NULL ,
   constraint fk_parking_lot FOREIGN KEY ("parking_lot_id")
   REFERENCES "public"."parking_lots"("id")
   ON UPDATE CASCADE
   ON DELETE NO ACTION,
  
   "incoming_datetime" timestamp with time zone NOT NULL,
   "outgoing_datetime" timestamp with time zone NOT NULL,
   "reservation_status" "text" default 'pending'
   CHECK (reservation_status IN (
       'pending', -- 予約待ち
       'confirmed', -- 予約確定
       'canceled', -- 予約キャンセル
       'completed' -- 予約完了(期日を過ぎた予約)
   )),
  
   "bus_campany_name" "text",
   "canceled_at" timestamp with time zone,
   "created_at" timestamp with time zone DEFAULT "now"() NOT NULL,
   "updated_at" timestamp with time zone DEFAULT "now"() NOT NULL
);
COMMENT ON TABLE "public"."reservations" IS '予約情報';
-- 複雑な関係性のFOREIGN KEY


-- パフォーマンス
-- CREATE INDEX
-- 都道府県での絞り込み検索
-- select * from public.parking_lots where prefecture_id = $1
CREATE INDEX parking_lots_prefecture_id_idx ON public.parking_lots USING btree (prefecture_id);
-- アカウントの予約一覧を取得する
-- select * from public.reservations where account_id = $1
CREATE INDEX reservations_account_id_idx ON public.reservations USING btree (account_id);
-- アカウントの特定の日の予約一覧を取得する
-- select * from public.reservations where account_id = $1 AND CAST(incoming AS DATE) = '2026-09-01';
CREATE INDEX reservations_account_id_incoming_idx ON public.reservations USING btree (account_id, incoming_datetime DESC);


-- CREATE STATISTICS
-- ロジック
--  CREATE FUNCTION
-- トリガー用の関数
-- updated_at フィールドを更新する
CREATE OR REPLACE FUNCTION "public"."update_timestamp"()
RETURNS TRIGGER AS $$
BEGIN
   NEW.updated_at = now();
   RETURN NEW;
END;
$$ LANGUAGE plpgsql;
--  CREATE PROCEDURE
--  イベント
--  CREATE TRIGGER
-- テーブルへのUPDATEを行う際にupdated_atの自動更新用トリガー
CREATE TRIGGER "trg_accounts_before_update_timestamp"
   BEFORE UPDATE ON "public"."accounts"
   FOR EACH ROW
   EXECUTE FUNCTION "public"."update_timestamp"();
CREATE TRIGGER "trg_accounts_profiles_before_update_timestamp"
   BEFORE UPDATE ON "public"."users"
   FOR EACH ROW
   EXECUTE FUNCTION "public"."update_timestamp"();
CREATE TRIGGER "trg_campanies_before_update_timestamp"
   BEFORE UPDATE ON "public"."campanies"
   FOR EACH ROW
   EXECUTE FUNCTION "public"."update_timestamp"();
CREATE TRIGGER "trg_parking_lots_before_update_timestamp"
   BEFORE UPDATE ON "public"."parking_lots"
   FOR EACH ROW
   EXECUTE FUNCTION "public"."update_timestamp"();
CREATE TRIGGER "trg_reservations_before_update_timestamp"
   BEFORE UPDATE ON "public"."reservations"
   FOR EACH ROW
   EXECUTE FUNCTION "public"."update_timestamp"();
-- セキュリティ
--  GRANT / REVOKE
--  ENABLE ROW LEVEL SECURITY
--  CREATE POLICY
