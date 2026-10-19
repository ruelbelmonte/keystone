create table if not exists customers (
    id         uuid primary key,
    name       varchar(200) not null,
    email      varchar(320) not null,
    created_at timestamptz  not null
);

-- Case-insensitive uniqueness; CustomerRepository maps a violation to 409.
create unique index if not exists customers_email_key on customers (lower(email));
