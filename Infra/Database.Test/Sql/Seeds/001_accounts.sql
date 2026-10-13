INSERT INTO "public"."accounts" 
(id, email, password_hash, password_salt, account_status, account_role)
VALUES 
('00000000-0000-0000-0000-000000000001', 'test1@example.com', '\x01020304', '\x04030201', 'just_created', 'user') ,
('00000000-0000-0000-0000-000000000002', 'test2@example.com', '\x01020304', '\x04030201', 'pending_email_verification', 'user') ,
('00000000-0000-0000-0000-000000000003', 'test3@example.com', '\x01020304', '\x04030201', 'onboarding', 'user') ,
('00000000-0000-0000-0000-000000000004', 'test4@example.com', '\x01020304', '\x04030201', 'activated', 'user') ,
('00000000-0000-0000-0000-000000000005', 'test5@example.com', '\x01020304', '\x04030201', 'suspended', 'user') ,
('00000000-0000-0000-0000-000000000006', 'test6@example.com', '\x01020304', '\x04030201', 'just_created', 'guest') ,
('00000000-0000-0000-0000-000000000007', 'test7@example.com', '\x01020304', '\x04030201', 'pending_email_verification', 'lot_admin') ,
('00000000-0000-0000-0000-000000000008', 'test8@example.com', '\x01020304', '\x04030201', 'activated', 'lot_admin') ,
('00000000-0000-0000-0000-000000000009', 'test9@example.com', '\x01020304', '\x04030201', 'activated', 'admin') ,
('00000000-0000-0000-0000-000000000010', 'test0@example.com', '\x01020304', '\x04030201', 'activated', 'admin') ;