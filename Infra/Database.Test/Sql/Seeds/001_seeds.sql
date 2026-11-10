INSERT INTO "public"."accounts" 
(id, email, password_hash, password_salt, account_status, account_role)
VALUES 
('00000000-0000-0000-0000-000000000001', 'test1@example.com', '\x01020304', '\x04030201', 'just_created', 'user') ,
('00000000-0000-0000-0000-000000000002', 'test2@example.com', '\x01020304', '\x04030201', 'pending_email_verification', 'user') ,
('00000000-0000-0000-0000-000000000003', 'test3@example.com', '\x01020304', '\x04030201', 'on_boarding', 'user') ,
('00000000-0000-0000-0000-000000000004', 'test4@example.com', '\x01020304', '\x04030201', 'activated', 'user') ,
('00000000-0000-0000-0000-000000000005', 'test5@example.com', '\x01020304', '\x04030201', 'suspended', 'user') ,
('00000000-0000-0000-0000-000000000006', 'test6@example.com', '\x01020304', '\x04030201', 'just_created', 'guest') ,
('00000000-0000-0000-0000-000000000007', 'test7@example.com', '\x01020304', '\x04030201', 'pending_email_verification', 'lot_admin') ,
('00000000-0000-0000-0000-000000000008', 'test8@example.com', '\x01020304', '\x04030201', 'activated', 'lot_admin') ,
('00000000-0000-0000-0000-000000000009', 'test9@example.com', '\x01020304', '\x04030201', 'activated', 'admin') ,
('00000000-0000-0000-0000-000000000010', 'test0@example.com', '\x01020304', '\x04030201', 'activated', 'admin') ;

INSERT INTO "public"."users"
(account_id, first_name, last_name, zip_code, street_address, phone_number, fax_number, user_type)
VALUES
('00000000-0000-0000-0000-000000000001',
'FirstName','LastName','1234567','東京都千代田区1-1-1',
'090-0000-0000','06-0000-0000', 'personal') ;



INSERT INTO parking_lots(
    id,
    display_name, prefecture_id, zip_code, street_address, phone_number, fax_number,
    max_capacity, reservation_capacity, is_allows_boarding_alighting, is_active
)
VALUES
('00000000-0000-0000-0000-000000000001',
'テスト駐車場', 1 , '1234567', '東京都千代田区1-1-1', '06-1234-5678', '06-4321-8765',
100, 50, false, true) ,
('00000000-0000-0000-0000-000000000002',
'Test駐車場', 1 , '1234567', '東京都千代田区1-1-1', '06-1234-5678', '06-4321-8765',
20, 10, true, true);