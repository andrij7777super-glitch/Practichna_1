# Звіт про виконання практичної роботи №5

## Наслідування та поліморфізм у системі Student Group Management System

| | |
|---|---|
| **Тема** | Наслідування та поліморфізм. Варіант 10 — система студентських заходів `UniversityEvent` |
| **Виконав** | Михальчук Андрій |
| **Група** | КН-21 |
| **Заклад** | Звягельський політехнічний фаховий коледж (ЗПФК) |
| **Технології** | C# 12, .NET 8, Console Application |
| **Папка проєкту** | `Praktuchna_5_Михальчук_Андрій` |

---

## Зміст

1. [Що таке наслідування?](#1-що-таке-наслідування)
2. [Приклади використання `base`](#2-приклади-використання-base)
3. [`virtual` / `override` та `new`](#3-virtual--override-та-new)
4. [Абстрактні класи](#4-абстрактні-класи)
5. [Ключове слово `sealed`](#5-ключове-слово-sealed)
6. [Поліморфізм у системі](#6-поліморфізм-у-системі)
7. [Труднощі проектування](#7-труднощі-проектування)
8. [Що нового дізналися](#8-що-нового-дізналися)
9. [Посилання на GitHub](#посилання-на-github-репозиторій)
10. [Скріншот git log](#скріншот-git-log)

---

## Структура ієрархії класів

У рамках Практичної роботи №5 було спроєктовано дві незалежні, але логічно пов'язані ієрархії:

```
Person (abstract)
 └── UniversityMember (abstract)
      └── Student
           ├── ExcellentStudent
           ├── ForeignStudent
           ├── WorkingStudent
           └── GraduateStudent (sealed)

UniversityEvent (abstract)
 ├── Hackathon
 ├── EsportsTournament
 └── ScienceConference
```

| Клас | Рівень | Призначення |
|------|--------|-------------|
| `Person` | Базовий | Загальні дані особи: ПІБ, email, вік |
| `UniversityMember` | Проміжний | Член університету: зарахування, стипендія |
| `Student` | Основний | Студент ЗПФК з оцінками, лабами, операторами |
| `ExcellentStudent` | Похідний | Підвищена стипендія для відмінників |
| `ForeignStudent` | Похідний | Іноземний студент із надбавкою |
| `WorkingStudent` | Похідний | Працюючий студент без стипендії |
| `GraduateStudent` | Кінцевий (`sealed`) | Випускник із темою диплома |
| `UniversityEvent` | Базовий (варіант 10) | Абстрактний студентський захід |
| `Hackathon` | Похідний | ІТ-хакатон |
| `EsportsTournament` | Похідний | Кіберспортивний турнір (CS2) |
| `ScienceConference` | Похідний | Наукова конференція |

> **Примітка.** Проєкт є **накопичувальним**: у класі `Student` та `StudentGroup` збережено функціонал Практичних робіт №1–4 (журнал оцінок, порти, CSV, оператори тощо), а ПР №5 додала ієрархію та поліморфну колекцію `List<UniversityMember>`.

---

## 1. Що таке наслідування?

**Наслідування** (inheritance) — фундаментальний принцип об'єктно-орієнтованого програмування, за яким один клас (похідний, *derived*) отримує поля, властивості та методи іншого класу (базового, *base*) і може їх **розширювати** або **перевизначати**.

У нашому проєкті ланцюжок наслідування виглядає так:

```text
Person → UniversityMember → Student → ExcellentStudent / ForeignStudent / ...
```

### Переваги наслідування (на прикладі СГМС)

| Перевага | Реалізація в коді |
|----------|-------------------|
| **Повторне використання коду** | Валідація `FullName` і `PersonalEmail` написана один раз у `Person` і успадковується всіма студентами |
| **Логічна структура предметної області** | «Особа → член університету → студент конкретного типу» відображає реальну ієрархію ЗПФК |
| **Розширюваність** | Новий тип студента (наприклад, `GraduateStudent`) додається без зміни базових класів |
| **Поліморфізм** | Один метод `CalculateScholarship()` поводиться по-різному для `ExcellentStudent`, `WorkingStudent` тощо |
| **Уніфікація інтерфейсу** | Метод `GetInfo()` формує звіт на кожному рівні, доповнюючи дані попереднього |

### Недоліки наслідування

| Недолік | Прояв у проєкті |
|---------|-----------------|
| **Жорсткий зв'язок класів** | `ExcellentStudent` неможливо використати без усієї ланцюжка `Person → UniversityMember → Student` |
| **Складність при глибоких ієрархіях** | Три рівні `GetInfo()` з викликами `base.GetInfo()` ускладнюють відстеження виводу |
| **Ризик порушення LSP** | Якщо похідний клас кардинально змінить семантику (наприклад, `WorkingStudent` завжди повертає стипендію `0`), потрібна обережність при роботі через базовий тип |
| **Складність рефакторингу** | Перенесення `RecordBookNumber` з `Student` у `UniversityMember` зачепило б усі методи пошуку з ПР №1–4 |
| **Проблема «крихкої бази»** | Зміна `Person.FullName` впливає на всі похідні класи одразу |

**Висновок:** наслідування в СГМС виправдане, оскільки відносини «студент — особа — член університету» є стабільними та природними для предметної області коледжу.

---

## 2. Приклади використання `base`

Ключове слово **`base`** у C# дає доступ до членів **безпосереднього базового класу**. У проєкті воно застосовано у **конструкторах** та **перевизначених методах**.

### 2.1. Виклик конструктора базового класу (`: base(...)`)

**`UniversityMember`** делегує ініціалізацію полів `Person`:

```csharp
protected UniversityMember(string fullName, DateTime dateOfBirth, string personalEmail, string notes)
    : base(fullName, dateOfBirth, personalEmail, notes)
{
}
```

**`Student`** передає параметри далі по ланцюжку:

```csharp
public Student(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber, string notes = "")
    : base(fullName, dateOfBirth, personalEmail, notes)
{
    _gradeJournal = new GradeJournal(SetAverageGradeInternal);
    RecordBookNumber = recordBookNumber;
    Status = StudentStatus.Active;
    GradePoints = new List<GradePoint>();
    Enroll();
}
```

**`ExcellentStudent`** використовує конструктор `Student`:

```csharp
public ExcellentStudent(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber, string notes = "")
    : base(fullName, dateOfBirth, personalEmail, recordBookNumber, notes)
{
}
```

Таким чином, при створенні відмінника **одним викликом** ініціалізуються поля всіх трьох рівнів: `Person` → `UniversityMember` → `Student` → `ExcellentStudent`.

### 2.2. Делегування логіки методів через `base.Method()`

**`Student.Enroll()`** — виклик базової реалізації з подальшим розширенням:

```csharp
public override void Enroll()
{
    base.Enroll();          // UniversityMember: встановлює EnrollmentDate
    Status = StudentStatus.Active;  // додаткова логіка студента
}
```

**`Student.GetInfo()`** — доповнення інформації базового класу:

```csharp
public override string GetInfo()
{
    var sb = new StringBuilder();
    sb.AppendLine(base.GetInfo());   // Person + UniversityMember
    sb.AppendLine($"Тип: {GetType().Name}");
    sb.AppendLine($"Залікова: {RecordBookNumber}");
    // ...
    return sb.ToString();
}
```

**`ExcellentStudent.CalculateScholarship()`** — комбінація власної та базової логіки:

```csharp
public override decimal CalculateScholarship() =>
    AverageGrade >= 90 ? 2500m : base.CalculateScholarship();
```

Якщо бал нижче 90, відмінник отримує **базову** стипендію `1500` грн (з класу `Student`), а не `0`.

**`ForeignStudent.CalculateScholarship()`** — надбавка до базової суми:

```csharp
public override decimal CalculateScholarship()
{
    var baseAmount = base.CalculateScholarship();
    return baseAmount == 0 ? 0m : baseAmount + 500m;
}
```

### 2.3. Ланцюжок `base.GetInfo()` на трьох рівнях

| Рівень | Клас | Що додає до виводу |
|--------|------|---------------------|
| 1 | `Person` | ПІБ, дата народження, вік, email, нотатки |
| 2 | `UniversityMember` | Дата зарахування |
| 3 | `Student` | Залікова, бал, прогрес, статус, стипендія |
| 4 | `ExcellentStudent` | Категорія «Відмінник» |

---

## 3. `virtual` / `override` та `new`

### 3.1. `virtual` + `override` — справжній поліморфізм

| Модифікатор | Де використано | Призначення |
|-------------|----------------|-------------|
| `virtual` | `Person.GetInfo()`, `UniversityMember.Enroll()` | Дозволяє нащадкам **перевизначити** метод |
| `override` | `Student.GetInfo()`, `ExcellentStudent.CalculateScholarship()` | Замінює реалізацію з урахуванням поліморфізму |
| `abstract` | `UniversityMember.CalculateScholarship()`, `UniversityEvent.ConductEvent()` | **Змушує** нащадка реалізувати метод |

**Приклад `virtual` у базовому класі:**

```csharp
// Person.cs
public virtual string GetInfo() { ... }
```

**Приклад `override` у похідному:**

```csharp
// UniversityMember.cs
public override string GetInfo()
{
    var sb = new StringBuilder();
    sb.Append(base.GetInfo());
    // ...
}
```

**Ключова властивість:** якщо змінна має тип `UniversityMember`, але посилається на об'єкт `ExcellentStudent`, виклик `member.CalculateScholarship()` виконає версію **відмінника**, а не базову:

```csharp
UniversityMember member = new ExcellentStudent(...);
decimal amount = member.CalculateScholarship(); // → 2500m (якщо бал ≥ 90)
```

Це демонструється в `StudentGroup.GetTotalScholarship()`:

```csharp
public decimal GetTotalScholarship() =>
    _members.Sum(m => m.CalculateScholarship());
```

### 3.2. `new` — приховування методу (method hiding)

Ключове слово **`new`** створює **новий** метод, який **приховує** базовий, але **не забезпечує поліморфізм**.

| Критерій | `override` | `new` |
|----------|------------|-------|
| Поліморфізм | ✅ Так | ❌ Ні |
| Виклик через базовий тип | Виконується похідна версія | Виконується базова версія |
| Потрібен `virtual`/`abstract` у базі | ✅ Так | ❌ Ні |
| Використання в СГМС | ✅ Для `GetInfo`, `CalculateScholarship`, `Enroll` | ❌ Свідомо **не** використовували |

**Ілюстративний приклад (не з проєкту, для порівняння):**

```csharp
public class Person
{
    public virtual void Print() => Console.WriteLine("Person");
}

public class Student : Person
{
    public override void Print() => Console.WriteLine("Student (override)"); // поліморфізм
}

public class BadExample : Person
{
    public new void Print() => Console.WriteLine("BadExample (new)"); // приховування
}
```

```csharp
Person p1 = new Student();
p1.Print(); // "Student (override)" — поліморфізм працює

Person p2 = new BadExample();
p2.Print(); // "Person" — new НЕ перевизначає віртуально!
```

У СГМС ми обрали **`override`** усюди, де потрібна поліморфна поведінка стипендій та звітів.

---

## 4. Абстрактні класи

**Абстрактний клас** — це клас, який **не можна інстанціювати** напряму (`new Person()` — помилка компіляції). Він задає **загальний каркас** і може містити як реалізовані, так і **абстрактні** методи без тіла.

### Де використано в проєкті

#### `Person` — абстрактний корінь ієрархії осіб

```csharp
public abstract class Person
{
    public virtual string GetInfo() { ... }  // реалізований метод
}
```

Містить спільну логіку валідації `FullName` та `PersonalEmail`. Не може бути створений окремо — лише через нащадків.

#### `UniversityMember` — абстрактний член університету

```csharp
public abstract class UniversityMember : Person
{
    public abstract decimal CalculateScholarship();  // ОБОВ'ЯЗКОВО реалізувати
    public virtual void Enroll() { ... }            // можна перевизначити
}
```

`CalculateScholarship()` — **абстрактний**, бо кожен тип члена університету має **унікальну** формулу стипендії. Базовий клас не знає, яку саме суму повертати.

#### `UniversityEvent` — абстрактний захід (варіант 10)

```csharp
public abstract class UniversityEvent
{
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public abstract void ConductEvent();
}
```

Метод `ConductEvent()` абстрактний, оскільки хакатон, кібертурнір і конференція проводяться **принципово по-різному**.

### Навіщо абстрактні класи, а не інтерфейси?

| Критерій | Абстрактний клас | Інтерфейс |
|----------|------------------|-----------|
| Спільні поля (`FullName`, `Email`) | ✅ Так | ❌ Ні (до C# 8) |
| Конструктор з валідацією | ✅ Так | ❌ Ні |
| Часткова реалізація (`GetInfo`) | ✅ Так | Обмежено |
| Множинне наслідування | ❌ Один базовий клас | ✅ Багато інтерфейсів |

Для СГМС абстрактні класи оптимальні: вони поєднують **спільний стан** (поля) і **спільну поведінку** (методи) з можливістю перевизначення.

---

## 5. Ключове слово `sealed`

### `sealed` для класу

Клас `GraduateStudent` позначено як **`sealed`** — це означає, що від нього **неможливо успадкуватися**:

```csharp
public sealed class GraduateStudent : Student
{
    public string ThesisTopic { get; set; } = string.Empty;

    public override decimal CalculateScholarship() => 3000m;
    // ...
}
```

**Навіщо це потрібно:**

| Причина | Пояснення |
|---------|-----------|
| **Завершальність ієрархії** | Випускник — кінцевий статус; нових підтипів «випускник-магістр» у рамках ПР не передбачено |
| **Захист інваріантів** | Фіксована стипендія `3000` грн і статус `Graduated` не повинні змінюватися нащадками |
| **Оптимізація компілятора** | `sealed` дозволяє JIT-компілятору де-віртуалізувати виклики методів |
| **Явний дизайн** | Сигналізує розробнику: «цей клас — листок дерева наслідування» |

Спроба створити `class SuperGraduate : GraduateStudent` призведе до **помилки CS0509**.

### `sealed` для методів

У проєкті `sealed` на методах не застосовували, але механізм полягає в тому, що `public sealed override void Method()` **забороняє** подальше перевизначення в нащадках. Це корисно, коли проміжний клас фіксує остаточну реалізацію.

---

## 6. Поліморфізм у системі

**Поліморфізм** — здатність об'єктів різних типів відповідати на однаковий виклик методу **різною** поведінкою. У СГМС реалізовано два яскраві приклади.

### 6.1. Поліморфна колекція `List<UniversityMember>`

У `StudentGroup` замість `List<Student>` використовується:

```csharp
private readonly List<UniversityMember> _members = new();
```

У ній одночасно зберігаються об'єкти різних типів:

| Об'єкт у тестових даних | Тип | Стипендія |
|-------------------------|-----|-----------|
| Коваленко Андрій | `Student` | 1500 грн |
| Бондаренко Марія | `ExcellentStudent` | 2500 грн |
| Schmidt Hans | `ForeignStudent` | 2000 грн (1500 + 500) |
| Шевченко Олена | `WorkingStudent` | 0 грн |

**Розрахунок загальної стипендії** — один цикл, різна логіка для кожного елемента:

```csharp
public decimal GetTotalScholarship() =>
    _members.Sum(m => m.CalculateScholarship());
```

**Виведення інформації** — поліморфний виклик `GetInfo()`:

```csharp
foreach (var member in _members)
{
    Console.WriteLine(member.GetInfo()); // кожен тип виводить свій формат
}
```

**Фільтрація за типом** — узагальнений метод:

```csharp
public List<T> GetMembersByType<T>() where T : UniversityMember =>
    _members.OfType<T>().ToList();
```

Приклад: `GetMembersByType<ForeignStudent>()` поверне лише іноземних студентів.

### 6.2. Таблиця реалізацій `CalculateScholarship()`

| Клас | Умова | Сума (грн) |
|------|-------|------------|
| `Student` | `AverageGrade >= 60` | 1500 |
| `Student` | `AverageGrade < 60` | 0 |
| `ExcellentStudent` | `AverageGrade >= 90` | 2500 |
| `ExcellentStudent` | `AverageGrade < 90` | `base` → 1500 або 0 |
| `ForeignStudent` | базова > 0 | базова + 500 |
| `WorkingStudent` | завжди | 0 |
| `GraduateStudent` | завжди | 3000 |

### 6.3. Поліморфізм заходів `ConductEvent()` (варіант 10)

Список заходів у `Program.cs`:

```csharp
List<UniversityEvent> events =
[
    new Hackathon { Title = "ZPFK CodeFest 2026", TechnologyStack = "C# / .NET 8, React, PostgreSQL" },
    new EsportsTournament { Title = "ZPFK Cyber Cup Spring 2026", GameTitle = "Counter-Strike 2", PrizePool = 15000m },
    new ScienceConference { Title = "Студентська наукова конференція ЗПФК", Topic = "Штучний інтелект у освіті" }
];
```

Один цикл — різна поведінка:

```csharp
foreach (var ev in events)
{
    ev.ConductEvent(); // Hackathon / EsportsTournament / ScienceConference
}
```

**`EsportsTournament.ConductEvent()`** — реальний приклад з тестових даних:

```csharp
public override void ConductEvent()
{
    Console.WriteLine($"=== КІБЕРСПОРТИВНИЙ ТУРНІР: {Title} ===");
    Console.WriteLine($"Гра: {GameTitle}");           // Counter-Strike 2
    Console.WriteLine($"Призовий фонд: {PrizePool:F2} грн");  // 15000.00 грн
    // ...
}
```

---

## 7. Труднощі проектування

### 7.1. Адаптація накопичувального функціоналу (ПР №1–4)

Головна труднощ — перехід від `List<Student> _students` до `List<UniversityMember> _members`. Старі методи очікували тип `Student` і ламалися б при наявності інших `UniversityMember`.

**Рішення — pattern matching та LINQ `OfType<T>()`:**

```csharp
// Пошук за заліковою (ПР №1)
public Student? FindStudent(string recordBookNumber) =>
    _members.OfType<Student>().FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

// Середній бал групи (ПР №1)
public double AverageGroupGrade
{
    get
    {
        var students = _members.OfType<Student>().ToList();
        return students.Count == 0 ? 0 : Math.Round(students.Average(s => s.AverageGrade), 2);
    }
}
```

**Експорт CSV (ПР №3)** — обробка різних типів учасників:

```csharp
foreach (var member in _members)
{
    if (member is Student student)
    {
        // повний рядок: залікова, бал, статус
        sb.AppendLine(student.GetType().Name);
    }
    else
    {
        // скорочений рядок для не-студентів
        sb.AppendLine(member.GetType().Name);
    }
}
```

**Прив'язка до порту (ПР №2)** — перевірка типу перед операцією:

```csharp
public void SimulateLabWork(string recordBookNumber, int labNumber, byte grade)
{
    if (FindStudent(recordBookNumber) is not Student student)
        throw new InvalidOperationException("Студента не знайдено.");
    // ...
    student.AddLabGrade(labNumber, grade);
}
```

### 7.2. Розміщення полів між рівнями ієрархії

| Поле | Рівень | Обґрунтування |
|------|--------|---------------|
| `FullName`, `Email` | `Person` | Спільне для всіх осіб |
| `EnrollmentDate` | `UniversityMember` | Лише для членів університету |
| `RecordBookNumber` | `Student` | Специфічне для студентів |
| `ThesisTopic` | `GraduateStudent` | Лише для випускників |

`RecordBookNumber` свідомо **не** піднято до `UniversityMember`, щоб не ускладнювати майбутні типи (наприклад, викладачів без залікової.

### 7.3. JSON-серіалізація поліморфної колекції

`System.Text.Json` не зберігає тип нащадка автоматично. **Рішення:** властивість `Students` для серіалізації, а `_members` — робоча колекція:

```csharp
[JsonInclude]
public List<Student> Students
{
    get => _members.OfType<Student>().ToList();
    set { /* синхронізація з _members */ }
}
```

### 7.4. Накопичувальне мега-меню

Об'єднання 24 пунктів меню з п'яти практичних робіт вимагало збереження всіх обробників без дублювання `Main`. Рішення — один `switch` у `Program.cs` з блоками ПР №1–5.

---

## 8. Що нового дізналися

Під час виконання Практичної роботи №5 було закріплено та опановано таке:

1. **Проектування багаторівневої ієрархії** — від абстрактного `Person` до конкретних `ExcellentStudent` і `sealed GraduateStudent`.

2. **Правильне використання `base`** — у конструкторах (`: base(...)`) та методах (`base.CalculateScholarship()`) для делегування, а не дублювання коду.

3. **Різницю між `override` і `new`** — для поліморфізму обов'язково `override`; `new` лише приховує метод без динамічної диспетчеризації.

4. **Абстрактні класи та методи** — як інструмент примусової реалізації (`CalculateScholarship`, `ConductEvent`) при збереженні спільного коду.

5. **`sealed`** — як спосіб зафіксувати кінцевий рівень ієрархії (`GraduateStudent`).

6. **Поліморфні колекції** — `List<UniversityMember>` з `OfType<T>()`, `is`-pattern matching та узагальненим `GetMembersByType<T>()`.

7. **Інтеграцію нової архітектури в існуючий код** — адаптація методів ПР №1–4 без їх видалення.

8. **Власний варіант 10** — ієрархія `UniversityEvent` з хакатоном, кібертурніром CS2 і науковою конференцією.

9. **Накопичувальний підхід до розробки** — одна кодова база, що еволюціонує від ПР №1 до №5, що відображає реальний життєвий цикл програмного проєкту.

---

## Додаток А. Фрагмент накопичувального меню

```text
--- Базові операції (ПР №1) ---       1–5
--- Масиви та Порти (ПР №2) ---        6–9
--- Робота з текстом (ПР №3) ---      10–14
--- Перевантаження операторів (ПР №4)  15–19
--- Наслідування та Поліморфізм (ПР №5) 20–24
```

---

## Додаток Б. Приклад тестових даних

```csharp
var excellent = new ExcellentStudent("Бондаренко Марія Сергіївна", ...);
excellent.Journal.AddOrUpdateGrade("Бази даних", 96);
excellent.GradePoints.Add(new GradePoint(9.5));

var foreign = new ForeignStudent("Schmidt Hans Peter", ..., "Німеччина", ...);
foreign.Journal.AddOrUpdateGrade("Англійська мова", 88);

var tournament = new EsportsTournament
{
    Title = "ZPFK Cyber Cup Spring 2026",
    GameTitle = "Counter-Strike 2",
    PrizePool = 15000m
};
```

---

## Посилання на GitHub репозиторій

> **Репозиторій проєкту:**
>
> ```
> https://github.com/ВАШ_ЛОГІН/StudentGroupApp-ZPFK
> ```
>
> *(вставте актуальне посилання на ваш репозиторій)*

---

## Скріншот git log

> Нижче — місце для вставки скріншота історії комітів (гілки Практичної роботи №5).
>
> ```
> ┌─────────────────────────────────────────────────────────────┐
> │                                                             │
> │   [ Вставте скріншот git log тут ]                          │
> │                                                             │
> │   Рекомендована команда:                                    │
> │   git log --oneline --graph --all                           │
> │                                                             │
> └─────────────────────────────────────────────────────────────┘
> ```

**Приклад очікуваних гілок/комітів:**

```text
* refactor/inheritance-hierarchy   — ієрархія Person → Student
* feature/university-events        — UniversityEvent, Hackathon, EsportsTournament
* feature/polymorphic-collection   — List<UniversityMember>, GetMembersByType
* feature/cumulative-menu          — мега-меню ПР №1–5
* main                             — фінальна збірка ПР №5
```

---

*Звіт підготовлено в рамках дисципліни «Об'єктно-орієнтоване програмування» для Звягельського політехнічного фахового коледжу (ЗПФК).*

**Михальчук Андрій · група КН-21 · 2026**
