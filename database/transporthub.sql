--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2 (Debian 15.2-1.pgdg110+1)
-- Dumped by pg_dump version 15.2

-- Started on 2023-04-21 01:40:30 CEST

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 3525 (class 1262 OID 17666)
-- Name: transporthub; Type: DATABASE; Schema: -; Owner: sysadmin
--

CREATE DATABASE transporthub WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE transporthub OWNER TO sysadmin;

\connect transporthub

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 5 (class 2615 OID 17667)
-- Name: conf; Type: SCHEMA; Schema: -; Owner: sysadmin
--

CREATE SCHEMA conf;


ALTER SCHEMA conf OWNER TO sysadmin;

--
-- TOC entry 6 (class 2615 OID 2200)
-- Name: orders; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

CREATE SCHEMA orders;


ALTER SCHEMA orders OWNER TO pg_database_owner;

--
-- TOC entry 3526 (class 0 OID 0)
-- Dependencies: 6
-- Name: SCHEMA orders; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA orders IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 228 (class 1259 OID 17739)
-- Name: clients; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.clients (
    id integer NOT NULL,
    name character varying NOT NULL,
    picture character varying,
    timezone timestamp with time zone NOT NULL,
    tin character varying NOT NULL,
    country character varying NOT NULL,
    location character varying NOT NULL,
    disabled integer DEFAULT 0 NOT NULL
);


ALTER TABLE conf.clients OWNER TO sysadmin;

--
-- TOC entry 3527 (class 0 OID 0)
-- Dependencies: 228
-- Name: COLUMN clients.location; Type: COMMENT; Schema: conf; Owner: sysadmin
--

COMMENT ON COLUMN conf.clients.location IS 'within a country';


--
-- TOC entry 230 (class 1259 OID 17747)
-- Name: clients_contact_info; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.clients_contact_info (
    id integer NOT NULL,
    client integer NOT NULL,
    contact_type integer NOT NULL,
    other_type_name character varying,
    value character varying NOT NULL,
    description character varying NOT NULL,
    disabled integer DEFAULT 0 NOT NULL
);


ALTER TABLE conf.clients_contact_info OWNER TO sysadmin;

--
-- TOC entry 229 (class 1259 OID 17746)
-- Name: clients_contact_info_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.clients_contact_info ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.clients_contact_info_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 227 (class 1259 OID 17738)
-- Name: clients_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.clients ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.clients_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 218 (class 1259 OID 17682)
-- Name: drivers; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.drivers (
    id integer NOT NULL,
    name character varying NOT NULL,
    picture character varying,
    nationality character varying NOT NULL,
    base_location character varying NOT NULL,
    disabled integer DEFAULT 0 NOT NULL
);


ALTER TABLE conf.drivers OWNER TO sysadmin;

--
-- TOC entry 217 (class 1259 OID 17681)
-- Name: drivers_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.drivers ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.drivers_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 224 (class 1259 OID 17706)
-- Name: drivers_licenses; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.drivers_licenses (
    id integer NOT NULL,
    driver integer NOT NULL,
    license integer NOT NULL,
    disabled integer DEFAULT 0 NOT NULL
);


ALTER TABLE conf.drivers_licenses OWNER TO sysadmin;

--
-- TOC entry 223 (class 1259 OID 17705)
-- Name: drivers_licenses_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.drivers_licenses ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.drivers_licenses_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 236 (class 1259 OID 17801)
-- Name: holidays; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.holidays (
    id integer NOT NULL,
    user_id integer,
    driver integer,
    reason character varying NOT NULL,
    approved integer NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    disabled integer NOT NULL,
    CONSTRAINT holidays_check CHECK ((((USER IS NULL) AND (driver IS NOT NULL)) OR ((USER IS NOT NULL) AND (driver IS NULL))))
);


ALTER TABLE conf.holidays OWNER TO sysadmin;

--
-- TOC entry 235 (class 1259 OID 17800)
-- Name: holidays_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.holidays ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.holidays_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 222 (class 1259 OID 17698)
-- Name: license_types; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.license_types (
    id integer NOT NULL,
    name character varying NOT NULL,
    description character varying NOT NULL,
    minimal_age_of_holder integer NOT NULL,
    alternative_minimal_age_of_holder integer,
    condition_for_alternative_minimal_age character varying,
    disabled integer DEFAULT 0 NOT NULL
);


ALTER TABLE conf.license_types OWNER TO sysadmin;

--
-- TOC entry 221 (class 1259 OID 17697)
-- Name: license_types_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.license_types ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.license_types_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 234 (class 1259 OID 17788)
-- Name: maintenance; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.maintenance (
    id integer NOT NULL,
    vehicle integer NOT NULL,
    reason character varying NOT NULL,
    from_transport integer NOT NULL,
    approved integer NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    disabled integer NOT NULL,
    vital integer NOT NULL
);


ALTER TABLE conf.maintenance OWNER TO sysadmin;

--
-- TOC entry 233 (class 1259 OID 17787)
-- Name: maintenance_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.maintenance ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.maintenance_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 226 (class 1259 OID 17722)
-- Name: preferred_vehicles; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.preferred_vehicles (
    id integer NOT NULL,
    driver integer NOT NULL,
    vehicle integer NOT NULL,
    priority integer DEFAULT 1 NOT NULL,
    disabled integer DEFAULT 0 NOT NULL
);


ALTER TABLE conf.preferred_vehicles OWNER TO sysadmin;

--
-- TOC entry 225 (class 1259 OID 17721)
-- Name: preferred_vehicles_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.preferred_vehicles ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.preferred_vehicles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 232 (class 1259 OID 17765)
-- Name: user_access_keys; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.user_access_keys (
    id integer NOT NULL,
    user_id integer NOT NULL,
    key character varying NOT NULL,
    last_refreshed timestamp with time zone NOT NULL,
    device_id character varying NOT NULL
);


ALTER TABLE conf.user_access_keys OWNER TO sysadmin;

--
-- TOC entry 231 (class 1259 OID 17764)
-- Name: user_access_keys_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.user_access_keys ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.user_access_keys_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 216 (class 1259 OID 17669)
-- Name: users; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.users (
    id integer NOT NULL,
    name character varying NOT NULL,
    picture character varying,
    password_salt character varying NOT NULL,
    password_hash character varying NOT NULL,
    password_expiration_date date,
    user_type smallint NOT NULL,
    multi_login integer DEFAULT 0 NOT NULL,
    disabled integer NOT NULL
);


ALTER TABLE conf.users OWNER TO sysadmin;

--
-- TOC entry 3528 (class 0 OID 0)
-- Dependencies: 216
-- Name: COLUMN users.disabled; Type: COMMENT; Schema: conf; Owner: sysadmin
--

COMMENT ON COLUMN conf.users.disabled IS 'false';


--
-- TOC entry 215 (class 1259 OID 17668)
-- Name: users_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.users ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 220 (class 1259 OID 17690)
-- Name: vehicles; Type: TABLE; Schema: conf; Owner: sysadmin
--

CREATE TABLE conf.vehicles (
    id integer NOT NULL,
    name character varying NOT NULL,
    vehicle_type smallint NOT NULL,
    picture character varying NOT NULL,
    required_license integer NOT NULL,
    registration_number character varying NOT NULL,
    vin character varying NOT NULL,
    disabled integer DEFAULT 0 NOT NULL
);


ALTER TABLE conf.vehicles OWNER TO sysadmin;

--
-- TOC entry 219 (class 1259 OID 17689)
-- Name: vehicles_id_seq; Type: SEQUENCE; Schema: conf; Owner: sysadmin
--

ALTER TABLE conf.vehicles ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME conf.vehicles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 248 (class 1259 OID 17950)
-- Name: assigned_users; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.assigned_users (
    id integer NOT NULL,
    user_id integer NOT NULL,
    order_id integer NOT NULL,
    is_primary integer NOT NULL
);


ALTER TABLE orders.assigned_users OWNER TO sysadmin;

--
-- TOC entry 247 (class 1259 OID 17949)
-- Name: assigned_users_id_seq; Type: SEQUENCE; Schema: orders; Owner: sysadmin
--

ALTER TABLE orders.assigned_users ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME orders.assigned_users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 246 (class 1259 OID 17932)
-- Name: documents; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.documents (
    id integer NOT NULL,
    order_id integer NOT NULL,
    transport integer,
    name character varying NOT NULL,
    description character varying NOT NULL,
    value bytea NOT NULL,
    document_type smallint NOT NULL
);


ALTER TABLE orders.documents OWNER TO sysadmin;

--
-- TOC entry 245 (class 1259 OID 17931)
-- Name: documents_id_seq; Type: SEQUENCE; Schema: orders; Owner: sysadmin
--

ALTER TABLE orders.documents ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME orders.documents_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 244 (class 1259 OID 17914)
-- Name: incident_vehicles; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.incident_vehicles (
    id integer NOT NULL,
    order_id integer NOT NULL,
    transport integer NOT NULL,
    incident integer NOT NULL,
    vehicle integer NOT NULL,
    needs_maintenance integer NOT NULL,
    maintenance_description character varying NOT NULL
);


ALTER TABLE orders.incident_vehicles OWNER TO sysadmin;

--
-- TOC entry 243 (class 1259 OID 17896)
-- Name: incidents; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.incidents (
    id integer NOT NULL,
    order_id integer NOT NULL,
    transport integer NOT NULL,
    description character varying NOT NULL,
    severity integer DEFAULT 1 NOT NULL,
    disabled integer NOT NULL
);


ALTER TABLE orders.incidents OWNER TO sysadmin;

--
-- TOC entry 239 (class 1259 OID 17825)
-- Name: orders; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.orders (
    id integer NOT NULL,
    key uuid NOT NULL,
    version integer NOT NULL,
    client integer NOT NULL,
    description character varying NOT NULL,
    initial_location character varying NOT NULL,
    final_location character varying NOT NULL,
    deadline_date timestamp with time zone NOT NULL,
    disabled integer NOT NULL,
    fulfilled integer NOT NULL,
    creation_date timestamp with time zone NOT NULL,
    created_by integer NOT NULL,
    modification_date timestamp with time zone NOT NULL,
    modified_by integer NOT NULL
);


ALTER TABLE orders.orders OWNER TO sysadmin;

--
-- TOC entry 249 (class 1259 OID 17975)
-- Name: latest_orders; Type: VIEW; Schema: orders; Owner: sysadmin
--

CREATE VIEW orders.latest_orders AS
 SELECT DISTINCT ON (o.key) o.id,
    o.key,
    o.version,
    o.client,
    o.description,
    o.initial_location,
    o.final_location,
    o.deadline_date,
    o.disabled,
    o.fulfilled,
    o.creation_date,
    o.created_by,
    o.modification_date,
    o.modified_by
   FROM orders.orders o
  ORDER BY o.key, o.modification_date DESC;


ALTER TABLE orders.latest_orders OWNER TO sysadmin;

--
-- TOC entry 237 (class 1259 OID 17819)
-- Name: orders_def; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.orders_def (
    key uuid NOT NULL
);


ALTER TABLE orders.orders_def OWNER TO sysadmin;

--
-- TOC entry 238 (class 1259 OID 17824)
-- Name: orders_id_seq; Type: SEQUENCE; Schema: orders; Owner: sysadmin
--

ALTER TABLE orders.orders ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME orders.orders_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 242 (class 1259 OID 17871)
-- Name: transport_vehicles; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.transport_vehicles (
    order_id integer NOT NULL,
    transport integer NOT NULL,
    id integer NOT NULL,
    vehicle integer NOT NULL,
    disabled integer NOT NULL
);


ALTER TABLE orders.transport_vehicles OWNER TO sysadmin;

--
-- TOC entry 241 (class 1259 OID 17859)
-- Name: transports; Type: TABLE; Schema: orders; Owner: sysadmin
--

CREATE TABLE orders.transports (
    order_id integer NOT NULL,
    id integer NOT NULL,
    departure_date timestamp with time zone NOT NULL,
    arrival_date timestamp with time zone NOT NULL,
    description character varying NOT NULL,
    driver integer NOT NULL,
    secondary_driver integer,
    disabled integer NOT NULL,
    fulfilled integer NOT NULL
);


ALTER TABLE orders.transports OWNER TO sysadmin;

--
-- TOC entry 240 (class 1259 OID 17858)
-- Name: transports_id_seq; Type: SEQUENCE; Schema: orders; Owner: sysadmin
--

ALTER TABLE orders.transports ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME orders.transports_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 3499 (class 0 OID 17739)
-- Dependencies: 228
-- Data for Name: clients; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3501 (class 0 OID 17747)
-- Dependencies: 230
-- Data for Name: clients_contact_info; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3489 (class 0 OID 17682)
-- Dependencies: 218
-- Data for Name: drivers; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3495 (class 0 OID 17706)
-- Dependencies: 224
-- Data for Name: drivers_licenses; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3507 (class 0 OID 17801)
-- Dependencies: 236
-- Data for Name: holidays; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3493 (class 0 OID 17698)
-- Dependencies: 222
-- Data for Name: license_types; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3505 (class 0 OID 17788)
-- Dependencies: 234
-- Data for Name: maintenance; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3497 (class 0 OID 17722)
-- Dependencies: 226
-- Data for Name: preferred_vehicles; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3503 (class 0 OID 17765)
-- Dependencies: 232
-- Data for Name: user_access_keys; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3487 (class 0 OID 17669)
-- Dependencies: 216
-- Data for Name: users; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3491 (class 0 OID 17690)
-- Dependencies: 220
-- Data for Name: vehicles; Type: TABLE DATA; Schema: conf; Owner: sysadmin
--



--
-- TOC entry 3519 (class 0 OID 17950)
-- Dependencies: 248
-- Data for Name: assigned_users; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3517 (class 0 OID 17932)
-- Dependencies: 246
-- Data for Name: documents; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3515 (class 0 OID 17914)
-- Dependencies: 244
-- Data for Name: incident_vehicles; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3514 (class 0 OID 17896)
-- Dependencies: 243
-- Data for Name: incidents; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3510 (class 0 OID 17825)
-- Dependencies: 239
-- Data for Name: orders; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3508 (class 0 OID 17819)
-- Dependencies: 237
-- Data for Name: orders_def; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3513 (class 0 OID 17871)
-- Dependencies: 242
-- Data for Name: transport_vehicles; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3512 (class 0 OID 17859)
-- Dependencies: 241
-- Data for Name: transports; Type: TABLE DATA; Schema: orders; Owner: sysadmin
--



--
-- TOC entry 3529 (class 0 OID 0)
-- Dependencies: 229
-- Name: clients_contact_info_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.clients_contact_info_id_seq', 1, false);


--
-- TOC entry 3530 (class 0 OID 0)
-- Dependencies: 227
-- Name: clients_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.clients_id_seq', 1, false);


--
-- TOC entry 3531 (class 0 OID 0)
-- Dependencies: 217
-- Name: drivers_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.drivers_id_seq', 1, false);


--
-- TOC entry 3532 (class 0 OID 0)
-- Dependencies: 223
-- Name: drivers_licenses_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.drivers_licenses_id_seq', 1, false);


--
-- TOC entry 3533 (class 0 OID 0)
-- Dependencies: 235
-- Name: holidays_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.holidays_id_seq', 1, false);


--
-- TOC entry 3534 (class 0 OID 0)
-- Dependencies: 221
-- Name: license_types_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.license_types_id_seq', 1, false);


--
-- TOC entry 3535 (class 0 OID 0)
-- Dependencies: 233
-- Name: maintenance_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.maintenance_id_seq', 1, false);


--
-- TOC entry 3536 (class 0 OID 0)
-- Dependencies: 225
-- Name: preferred_vehicles_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.preferred_vehicles_id_seq', 1, false);


--
-- TOC entry 3537 (class 0 OID 0)
-- Dependencies: 231
-- Name: user_access_keys_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.user_access_keys_id_seq', 1, false);


--
-- TOC entry 3538 (class 0 OID 0)
-- Dependencies: 215
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.users_id_seq', 3, true);


--
-- TOC entry 3539 (class 0 OID 0)
-- Dependencies: 219
-- Name: vehicles_id_seq; Type: SEQUENCE SET; Schema: conf; Owner: sysadmin
--

SELECT pg_catalog.setval('conf.vehicles_id_seq', 1, false);


--
-- TOC entry 3540 (class 0 OID 0)
-- Dependencies: 247
-- Name: assigned_users_id_seq; Type: SEQUENCE SET; Schema: orders; Owner: sysadmin
--

SELECT pg_catalog.setval('orders.assigned_users_id_seq', 1, false);


--
-- TOC entry 3541 (class 0 OID 0)
-- Dependencies: 245
-- Name: documents_id_seq; Type: SEQUENCE SET; Schema: orders; Owner: sysadmin
--

SELECT pg_catalog.setval('orders.documents_id_seq', 1, false);


--
-- TOC entry 3542 (class 0 OID 0)
-- Dependencies: 238
-- Name: orders_id_seq; Type: SEQUENCE SET; Schema: orders; Owner: sysadmin
--

SELECT pg_catalog.setval('orders.orders_id_seq', 1, false);


--
-- TOC entry 3543 (class 0 OID 0)
-- Dependencies: 240
-- Name: transports_id_seq; Type: SEQUENCE SET; Schema: orders; Owner: sysadmin
--

SELECT pg_catalog.setval('orders.transports_id_seq', 1, false);


--
-- TOC entry 3293 (class 2606 OID 17753)
-- Name: clients_contact_info clients_contact_info_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.clients_contact_info
    ADD CONSTRAINT clients_contact_info_pk PRIMARY KEY (id);


--
-- TOC entry 3291 (class 2606 OID 17745)
-- Name: clients clients_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.clients
    ADD CONSTRAINT clients_pk PRIMARY KEY (id);


--
-- TOC entry 3287 (class 2606 OID 17710)
-- Name: drivers_licenses drivers_licenses_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.drivers_licenses
    ADD CONSTRAINT drivers_licenses_pk PRIMARY KEY (id);


--
-- TOC entry 3281 (class 2606 OID 17688)
-- Name: drivers drivers_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.drivers
    ADD CONSTRAINT drivers_pk PRIMARY KEY (id);


--
-- TOC entry 3299 (class 2606 OID 17808)
-- Name: holidays holidays_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.holidays
    ADD CONSTRAINT holidays_pk PRIMARY KEY (id);


--
-- TOC entry 3285 (class 2606 OID 17704)
-- Name: license_types license_types_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.license_types
    ADD CONSTRAINT license_types_pk PRIMARY KEY (id);


--
-- TOC entry 3297 (class 2606 OID 17794)
-- Name: maintenance maintenance_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.maintenance
    ADD CONSTRAINT maintenance_pk PRIMARY KEY (id);


--
-- TOC entry 3289 (class 2606 OID 17727)
-- Name: preferred_vehicles preferred_vehicles_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.preferred_vehicles
    ADD CONSTRAINT preferred_vehicles_pk PRIMARY KEY (id);


--
-- TOC entry 3295 (class 2606 OID 17771)
-- Name: user_access_keys user_access_keys_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.user_access_keys
    ADD CONSTRAINT user_access_keys_pk PRIMARY KEY (id);


--
-- TOC entry 3279 (class 2606 OID 17680)
-- Name: users users_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.users
    ADD CONSTRAINT users_pk PRIMARY KEY (id);


--
-- TOC entry 3283 (class 2606 OID 17696)
-- Name: vehicles vehicles_pk; Type: CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.vehicles
    ADD CONSTRAINT vehicles_pk PRIMARY KEY (id);


--
-- TOC entry 3315 (class 2606 OID 17954)
-- Name: assigned_users assigned_users_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.assigned_users
    ADD CONSTRAINT assigned_users_pk PRIMARY KEY (id);


--
-- TOC entry 3313 (class 2606 OID 17938)
-- Name: documents documents_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.documents
    ADD CONSTRAINT documents_pk PRIMARY KEY (id);


--
-- TOC entry 3311 (class 2606 OID 17920)
-- Name: incident_vehicles incident_vehicles_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.incident_vehicles
    ADD CONSTRAINT incident_vehicles_pk PRIMARY KEY (id, order_id, transport, incident);


--
-- TOC entry 3309 (class 2606 OID 17913)
-- Name: incidents incidents_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.incidents
    ADD CONSTRAINT incidents_pk PRIMARY KEY (id, order_id, transport);


--
-- TOC entry 3301 (class 2606 OID 17823)
-- Name: orders_def orders_def_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.orders_def
    ADD CONSTRAINT orders_def_pk PRIMARY KEY (key);


--
-- TOC entry 3303 (class 2606 OID 17831)
-- Name: orders orders_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.orders
    ADD CONSTRAINT orders_pk PRIMARY KEY (id);


--
-- TOC entry 3307 (class 2606 OID 17875)
-- Name: transport_vehicles transport_vehicles_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.transport_vehicles
    ADD CONSTRAINT transport_vehicles_pk PRIMARY KEY (order_id, transport, id);


--
-- TOC entry 3305 (class 2606 OID 17865)
-- Name: transports transports_pk; Type: CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.transports
    ADD CONSTRAINT transports_pk PRIMARY KEY (id, order_id);


--
-- TOC entry 3321 (class 2606 OID 17754)
-- Name: clients_contact_info clients_contact_info_fk; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.clients_contact_info
    ADD CONSTRAINT clients_contact_info_fk FOREIGN KEY (client) REFERENCES conf.clients(id);


--
-- TOC entry 3317 (class 2606 OID 17711)
-- Name: drivers_licenses drivers_licenses_fk_driver; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.drivers_licenses
    ADD CONSTRAINT drivers_licenses_fk_driver FOREIGN KEY (driver) REFERENCES conf.drivers(id);


--
-- TOC entry 3318 (class 2606 OID 17716)
-- Name: drivers_licenses drivers_licenses_fk_license; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.drivers_licenses
    ADD CONSTRAINT drivers_licenses_fk_license FOREIGN KEY (license) REFERENCES conf.license_types(id);


--
-- TOC entry 3324 (class 2606 OID 17809)
-- Name: holidays holidays_fk_driver; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.holidays
    ADD CONSTRAINT holidays_fk_driver FOREIGN KEY (driver) REFERENCES conf.drivers(id);


--
-- TOC entry 3325 (class 2606 OID 17814)
-- Name: holidays holidays_fk_user; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.holidays
    ADD CONSTRAINT holidays_fk_user FOREIGN KEY (user_id) REFERENCES conf.users(id);


--
-- TOC entry 3323 (class 2606 OID 17795)
-- Name: maintenance maintenance_fk; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.maintenance
    ADD CONSTRAINT maintenance_fk FOREIGN KEY (vehicle) REFERENCES conf.vehicles(id);


--
-- TOC entry 3319 (class 2606 OID 17728)
-- Name: preferred_vehicles preferred_vehicles_fk_driver; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.preferred_vehicles
    ADD CONSTRAINT preferred_vehicles_fk_driver FOREIGN KEY (driver) REFERENCES conf.drivers(id);


--
-- TOC entry 3320 (class 2606 OID 17733)
-- Name: preferred_vehicles preferred_vehicles_fk_vehicle; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.preferred_vehicles
    ADD CONSTRAINT preferred_vehicles_fk_vehicle FOREIGN KEY (vehicle) REFERENCES conf.vehicles(id);


--
-- TOC entry 3322 (class 2606 OID 17772)
-- Name: user_access_keys user_access_keys_fk; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.user_access_keys
    ADD CONSTRAINT user_access_keys_fk FOREIGN KEY (user_id) REFERENCES conf.users(id);


--
-- TOC entry 3316 (class 2606 OID 17759)
-- Name: vehicles vehicles_fk; Type: FK CONSTRAINT; Schema: conf; Owner: sysadmin
--

ALTER TABLE ONLY conf.vehicles
    ADD CONSTRAINT vehicles_fk FOREIGN KEY (required_license) REFERENCES conf.license_types(id);


--
-- TOC entry 3341 (class 2606 OID 17955)
-- Name: assigned_users assigned_users_fk_orders; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.assigned_users
    ADD CONSTRAINT assigned_users_fk_orders FOREIGN KEY (order_id) REFERENCES orders.orders(id);


--
-- TOC entry 3342 (class 2606 OID 17960)
-- Name: assigned_users assigned_users_fk_users; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.assigned_users
    ADD CONSTRAINT assigned_users_fk_users FOREIGN KEY (user_id) REFERENCES conf.users(id);


--
-- TOC entry 3339 (class 2606 OID 17944)
-- Name: documents documents_fk_orders; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.documents
    ADD CONSTRAINT documents_fk_orders FOREIGN KEY (order_id) REFERENCES orders.orders(id);


--
-- TOC entry 3340 (class 2606 OID 17939)
-- Name: documents documents_fk_transports; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.documents
    ADD CONSTRAINT documents_fk_transports FOREIGN KEY (transport, order_id) REFERENCES orders.transports(id, order_id);


--
-- TOC entry 3337 (class 2606 OID 17921)
-- Name: incident_vehicles incident_vehicles_fk_incidents; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.incident_vehicles
    ADD CONSTRAINT incident_vehicles_fk_incidents FOREIGN KEY (incident, order_id, transport) REFERENCES orders.incidents(id, order_id, transport);


--
-- TOC entry 3338 (class 2606 OID 17926)
-- Name: incident_vehicles incident_vehicles_fk_vehicles; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.incident_vehicles
    ADD CONSTRAINT incident_vehicles_fk_vehicles FOREIGN KEY (vehicle) REFERENCES conf.vehicles(id);


--
-- TOC entry 3335 (class 2606 OID 17902)
-- Name: incidents incidents_fk; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.incidents
    ADD CONSTRAINT incidents_fk FOREIGN KEY (order_id) REFERENCES orders.orders(id);


--
-- TOC entry 3336 (class 2606 OID 17907)
-- Name: incidents incidents_fk_transports; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.incidents
    ADD CONSTRAINT incidents_fk_transports FOREIGN KEY (transport, order_id) REFERENCES orders.transports(id, order_id);


--
-- TOC entry 3326 (class 2606 OID 17837)
-- Name: orders orders_fk_client; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.orders
    ADD CONSTRAINT orders_fk_client FOREIGN KEY (client) REFERENCES conf.clients(id);


--
-- TOC entry 3327 (class 2606 OID 17965)
-- Name: orders orders_fk_created_by; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.orders
    ADD CONSTRAINT orders_fk_created_by FOREIGN KEY (created_by) REFERENCES conf.users(id);


--
-- TOC entry 3328 (class 2606 OID 17832)
-- Name: orders orders_fk_def; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.orders
    ADD CONSTRAINT orders_fk_def FOREIGN KEY (key) REFERENCES orders.orders_def(key);


--
-- TOC entry 3329 (class 2606 OID 17970)
-- Name: orders orders_fk_modified_by; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.orders
    ADD CONSTRAINT orders_fk_modified_by FOREIGN KEY (modified_by) REFERENCES conf.users(id);


--
-- TOC entry 3333 (class 2606 OID 17876)
-- Name: transport_vehicles transport_vehicles_fk_transports; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.transport_vehicles
    ADD CONSTRAINT transport_vehicles_fk_transports FOREIGN KEY (transport, order_id) REFERENCES orders.transports(id, order_id);


--
-- TOC entry 3334 (class 2606 OID 17881)
-- Name: transport_vehicles transport_vehicles_fk_vehicles; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.transport_vehicles
    ADD CONSTRAINT transport_vehicles_fk_vehicles FOREIGN KEY (vehicle) REFERENCES conf.vehicles(id);


--
-- TOC entry 3330 (class 2606 OID 17866)
-- Name: transports transports_fk; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.transports
    ADD CONSTRAINT transports_fk FOREIGN KEY (order_id) REFERENCES orders.orders(id);


--
-- TOC entry 3331 (class 2606 OID 17886)
-- Name: transports transports_fk_primary_driver; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.transports
    ADD CONSTRAINT transports_fk_primary_driver FOREIGN KEY (driver) REFERENCES conf.drivers(id);


--
-- TOC entry 3332 (class 2606 OID 17891)
-- Name: transports transports_fk_secondary_driver; Type: FK CONSTRAINT; Schema: orders; Owner: sysadmin
--

ALTER TABLE ONLY orders.transports
    ADD CONSTRAINT transports_fk_secondary_driver FOREIGN KEY (secondary_driver) REFERENCES conf.drivers(id);


-- Completed on 2023-04-21 01:40:35 CEST

--
-- PostgreSQL database dump complete
--

