INSERT INTO "public"."accounts" 
(email, password_hash, password_salt, account_status, account_role)
VALUES 
('test1@example.com', '\x01020304', '\x04030201', 'just_created', 'user') ,
('test2@example.com', '\x01020304', '\x04030201', 'pending_email_verification', 'user') ,
('test3@example.com', '\x01020304', '\x04030201', 'onboarding', 'user') ,
('test4@example.com', '\x01020304', '\x04030201', 'activated', 'user') ,
('test5@example.com', '\x01020304', '\x04030201', 'suspended', 'user') ,
('test6@example.com', '\x01020304', '\x04030201', 'just_created', 'guest') ,
('test7@example.com', '\x01020304', '\x04030201', 'pending_email_verification', 'lot_admin') ,
('test8@example.com', '\x01020304', '\x04030201', 'activated', 'lot_admin') ,
('test9@example.com', '\x01020304', '\x04030201', 'activated', 'admin') ,
('test0@example.com', '\x01020304', '\x04030201', 'activated', 'admin') ;