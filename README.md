# Курсов проект: ASP.NET Web API на лесна тема — Система за библиотека

Този документ описва лесна, но достатъчно богата тема за курсов проект, която покрива всички изисквания. Проектът може да се реализира в два варианта:

1. **Самостоятелно Web API + Postman Collection** (по-строгите изисквания).
2. **Web API + JavaScript front-end** (React/Angular/друг, генериран с AI).

По-долу са описани **домейн модел**, **таблици**, **връзки**, **функционалности**, **endpoints**, както и **happy path** сценарии.

---

## Структура на проекта
- `LibraryApi/` — ASP.NET Web API (EF Core + SQLite).
- `library-frontend/` — минимален JavaScript фронт-енд (AI-генериран).

---

## Стартиране

### 1) Backend (ASP.NET Web API)
```bash
cd LibraryApi
dotnet restore
dotnet ef database update
dotnet run
```

По подразбиране API работи на `https://localhost:5001`.

### 2) Front-end
Отворете `library-frontend/index.html` в браузър и сменете `apiBase` в `app.js`, ако използвате различен адрес.

### 3) Postman Collection
Колекцията е в `postman/LibraryApi.postman_collection.json` и използва променлива `baseUrl`.

---

## 1) Тема и цел
**Тема:** *Система за управление на библиотека*  
**Цел:** Управление на книги, автори, жанрове, издатели, членове, заеми, резервации и плащания на глоби.

Това е лесна за разбиране тема с ясни бизнес процеси и достатъчно релации.

---

## 2) Таблици (9+) и връзки

### Таблици (предложени 10)
1. **Books** (Книги)
2. **Authors** (Автори)
3. **Publishers** (Издатели)
4. **Genres** (Жанрове)
5. **Members** (Читатели)
6. **Loans** (Заеми)
7. **Reservations** (Резервации)
8. **Payments** (Плащания/глоби)
9. **Branches** (Библиотечни филиали)
10. **BookCopies** (Конкретни копия на книга)

### Връзки 1→N (поне 6)
1. **Publishers (1) → Books (N)**
2. **Books (1) → BookCopies (N)**
3. **Members (1) → Loans (N)**
4. **Members (1) → Reservations (N)**
5. **Loans (1) → Payments (N)**
6. **Branches (1) → BookCopies (N)**

### Връзки M↔N (поне 4)
1. **Books ↔ Authors** (таблица BookAuthors)
2. **Books ↔ Genres** (таблица BookGenres)
3. **Members ↔ Books** чрез **Reservations** (една книга може да е резервирана от много членове, а член може да резервира много книги)
4. **Members ↔ Books** чрез **Loans** (много към много през заемите)

> Забележка: За M↔N при Loans/Reservations имаме “асоциативни” таблици, които са отделни функционалности (не само join таблици), което е напълно приемливо.

---

## 3) Минимален набор от атрибути (пример)

### Books
- Id, Title, ISBN, PublishYear, PublisherId

### Authors
- Id, FirstName, LastName

### Publishers
- Id, Name, City

### Genres
- Id, Name

### Members
- Id, FullName, Email, Phone, JoinDate

### BookCopies
- Id, BookId, BranchId, InventoryCode, Status (Available/Loaned/Reserved)

### Loans
- Id, MemberId, BookCopyId, LoanDate, DueDate, ReturnDate

### Reservations
- Id, MemberId, BookId, ReservedAt, Status (Active/Cancelled/Fulfilled)

### Payments
- Id, LoanId, Amount, PaidAt, Method (Cash/Card)

### Branches
- Id, Name, Address

### Join таблици
- **BookAuthors** (BookId, AuthorId)
- **BookGenres** (BookId, GenreId)

---

## 4) Основна функционалност (работа с всички таблици)

### Books
- CRUD за книги
- Добавяне/премахване на автори и жанрове

### Authors
- CRUD за автори

### Publishers
- CRUD за издатели

### Genres
- CRUD за жанрове

### Members
- CRUD за читатели

### BookCopies
- CRUD за копия на книги
- Прехвърляне на копия към филиал

### Loans
- Създаване на заем (само ако копието е налично)
- Връщане на заем (ReturnDate, промяна на статус)

### Reservations
- Създаване на резервация за книга
- Отмяна/изпълнение на резервация

### Payments
- Плащане на глоба за заем

### Branches
- CRUD за филиали

---

## 5) Примерни API Endpoints (happy path)

### Books
- `POST /api/books` (създава книга)
- `GET /api/books/{id}`
- `PUT /api/books/{id}`
- `DELETE /api/books/{id}`
- `POST /api/books/{id}/authors/{authorId}`
- `POST /api/books/{id}/genres/{genreId}`

### Authors, Genres, Publishers, Members, Branches
- стандартни CRUD endpoints

### BookCopies
- `POST /api/bookcopies`
- `PUT /api/bookcopies/{id}/move/{branchId}`

### Loans
- `POST /api/loans` (създава заем за конкретно копие)
- `PUT /api/loans/{id}/return`

### Reservations
- `POST /api/reservations`
- `PUT /api/reservations/{id}/cancel`
- `PUT /api/reservations/{id}/fulfill`

### Payments
- `POST /api/payments` (плащане на глоба по заем)

---

## 6) Happy path сценарии (по една линия за всяка функционалност)
1. **Създаване на издател → книга → копие** (Publisher → Book → BookCopy).
2. **Добавяне на автор и жанр към книга.**
3. **Регистрация на читател.**
4. **Създаване на заем за налично копие и връщане на книга.**
5. **Резервация на книга и изпълнение на резервацията.**
6. **Плащане на глоба към заем.**
7. **Прехвърляне на копие към друг филиал.**

---

## 7) Postman Collection (за варианта без front-end)
Създайте колекция с заявки за:
1. CRUD за всяка таблица.
2. Добавяне на автори/жанрове към книга.
3. Създаване и връщане на заем.
4. Резервация и изпълнение.
5. Плащане на глоба.

> Това гарантира пълна функционалност и покритие на всички таблици.

---

## 8) Front-end вариант (ако се избере)
Минимален UI:
- Списък с книги + детайли
- Форма за създаване на заем/резервация
- Форма за читатели
- Форма за плащане

---

## 9) Защо темата е “лесна”
1. Добре познат домейн (книги/заеми).
2. Ясни бизнес правила (налично копие, срок за заем).
3. Лесни за обяснение релации.

---

## 10) Минимален план за реализация
1. **Проектиране на база** (EF Core миграции).
2. **CRUD endpoints** за всички таблици.
3. **Бизнес логика**: заем, връщане, резервации.
4. **Postman колекция** или **front-end**.
5. **Тестове на happy path**.

---

Ако желаеш, мога да подготвя и:
- примерен SQL/EF Core модел,
- пълна Postman колекция,
- или фронтенд шаблон.
