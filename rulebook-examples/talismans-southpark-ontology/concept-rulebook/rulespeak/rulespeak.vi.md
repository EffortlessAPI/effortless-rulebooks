# 📘 Talisman's South Park ontology explainer: the concept form — Tài liệu

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Các quy tắc nghiệp vụ dạng khai báo, được sinh ra từ bộ quy tắc. Mỗi phát biểu
> dưới đây diễn đạt một sự thật trong lĩnh vực nghiệp vụ — không phải là một thủ tục,
> cũng không phải là một mệnh lệnh. Các công thức của bộ quy tắc là nguồn sự thật duy nhất;
> tài liệu này là cách đọc các công thức đó bằng lời thông thường.

## 1 Từ vựng nghiệp vụ

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Thuộc tính đã được định nghĩa. | _The name the page shows._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the cast._ |
| Portrait | Thuộc tính đã được định nghĩa. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Thuộc tính đã được định nghĩa. | _The individual's row in Things._ |
| Is Asserted | Đúng khi chuỗi rỗng. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Thuộc tính đã được định nghĩa. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Thuộc tính đã được định nghĩa. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Thuộc tính đã được định nghĩa. | _The individual's row in Things._ |
| Is Asserted | Đúng khi chuỗi rỗng. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Is Asserted |
|---|
| false |

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Thuộc tính đã được định nghĩa. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Thuộc tính đã được định nghĩa. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Thuộc tính đã được định nghĩa. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Thuộc tính đã được định nghĩa. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Thuộc tính đã được định nghĩa. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Thuộc tính đã được định nghĩa. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Thuộc tính đã được định nghĩa. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Thuộc tính đã được định nghĩa. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Thuộc tính đã được định nghĩa. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Thuộc tính đã được định nghĩa. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Thuộc tính đã được định nghĩa. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Thuộc tính đã được định nghĩa. | _The value._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Value |
|---|
| 10 |

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Thuộc tính đã được định nghĩa. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Thuộc tính đã được định nghĩa. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Thuộc tính đã được định nghĩa. | _The name on the portrait's Fandom page._ |
| Page | Thuộc tính đã được định nghĩa. | _The Fandom page credited for the portrait._ |
| Image | Thuộc tính đã được định nghĩa. | _The original Fandom image._ |
| Crop | Thuộc tính đã được định nghĩa. | _The crop as an SVG viewBox: x y width height._ |
| Width | Thuộc tính đã được định nghĩa. | _The original image's width in pixels._ |
| Height | Thuộc tính đã được định nghĩa. | _The original image's height in pixels._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the credits._ |
| Name | Có cùng giá trị với trường “display name” của chính portrait đó. | _Order 1. The portrait's name._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Thuộc tính đã được định nghĩa. | _The table's name again, as a plain column._ |
| Description | Thuộc tính đã được định nghĩa. | _What the table holds. Follows the table._ |
| Is Meta Table | Đúng khi chuỗi rỗng. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Đúng khi chuỗi rỗng. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Thuộc tính đã được định nghĩa. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Thuộc tính đã được định nghĩa. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Thuộc tính đã được định nghĩa. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Thuộc tính đã được định nghĩa. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Đúng khi chuỗi rỗng. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Đúng khi chuỗi rỗng. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Thuộc tính đã được định nghĩa. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Thuộc tính đã được định nghĩa. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Thuộc tính đã được định nghĩa. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Thuộc tính đã được định nghĩa. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Thuộc tính đã được định nghĩa. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Thuộc tính đã được định nghĩa. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Thuộc tính đã được định nghĩa. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Thuộc tính đã được định nghĩa. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Thuộc tính đã được định nghĩa. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Thuộc tính đã được định nghĩa. | _The table the field is on._ |
| Field Name | Thuộc tính đã được định nghĩa. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Thuộc tính đã được định nghĩa. | _raw or relationship. Follows the field._ |
| Datatype | Thuộc tính đã được định nghĩa. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Đúng khi chuỗi rỗng. | _True for the field that identifies a row of its table._ |
| Description | Thuộc tính đã được định nghĩa. | _What the field means. Follows the field._ |
| Label | Thuộc tính đã được định nghĩa. | _For a property: The property's wording on the page._ |
| Is User Addable | Đúng khi chuỗi rỗng. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Đúng khi chuỗi rỗng. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Thuộc tính đã được định nghĩa. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Thuộc tính đã được định nghĩa. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Thuộc tính đã được định nghĩa. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Thuộc tính đã được định nghĩa. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Thuộc tính đã được định nghĩa. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Thuộc tính đã được định nghĩa. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Thuộc tính đã được định nghĩa. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Thuộc tính đã được định nghĩa. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Thuộc tính đã được định nghĩa. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Kiểu dữ kiện

- Mỗi **thing** có thể tham chiếu đến một **portrait**
- Mỗi **person** tham chiếu đến đúng một **thing**
- Mỗi **school** tham chiếu đến đúng một **thing**
- Mỗi **is parent of** tham chiếu đến đúng một **thing**
- Mỗi **is child of** tham chiếu đến đúng một **thing**
- Mỗi **is spouse of** tham chiếu đến đúng một **thing**
- Mỗi **is friend of** tham chiếu đến đúng một **thing**
- Mỗi **attend** tham chiếu đến đúng một **person**
- Mỗi **attend** tham chiếu đến đúng một **school**
- Mỗi **has age** tham chiếu đến đúng một **thing**
- Mỗi **is grandparent of** tham chiếu đến đúng một **thing**
- Mỗi **ERB table** có thể tham chiếu đến một **ERB table**
- Mỗi **ERB table** có thể tham chiếu đến một **portrait**
- Mỗi **ERB field** có thể tham chiếu đến một **ERB table**

## 3 Quy tắc tác nghiệp

_Quy tắc tác nghiệp nêu những điều mà doanh nghiệp **bắt buộc**, **nghiêm cấm** hoặc
khuyến nghị (**nên**). Quy tắc cấu trúc bắt nguồn từ các trường bắt buộc và khóa ngoại;
quy tắc ngữ nghĩa bắt nguồn từ bảng Constraints, mỗi quy tắc căn cứ vào một giá trị logic mà bộ quy tắc
đã tính sẵn (được tham chiếu chéo dưới dạng DR-N trong phần Quy tắc định nghĩa bên dưới)._

### Ràng buộc cấu trúc (từ lược đồ)

- Mỗi thing **phải** có label và sort order.
- Mỗi person **phải** tham chiếu đến đúng một thing.
- Mỗi person **phải** được ghi nhận rõ là “asserted” hay không.
- Mỗi school **phải** tham chiếu đến đúng một thing.
- Mỗi school **phải** được ghi nhận rõ là “asserted” hay không.
- Mỗi is parent of **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi is parent of **phải** tham chiếu đến đúng một thing với vai trò object.
- Mỗi is child of **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi is child of **phải** tham chiếu đến đúng một thing với vai trò object.
- Mỗi is spouse of **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi is spouse of **phải** tham chiếu đến đúng một thing với vai trò object.
- Mỗi is friend of **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi is friend of **phải** tham chiếu đến đúng một thing với vai trò object.
- Mỗi attend **phải** tham chiếu đến đúng một person với vai trò subject.
- Mỗi attend **phải** tham chiếu đến đúng một school với vai trò object.
- Mỗi has age **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi has age **phải** có value.
- Mỗi is grandparent of **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi is grandparent of **phải** tham chiếu đến đúng một thing với vai trò object.
- Mỗi portrait **phải** có display name, page, image, crop, width, height và sort order.

## 4 Quy tắc định nghĩa

_Mọi phát biểu đều diễn đạt sự thật trong lĩnh vực nghiệp vụ; chúng không phải là
thủ tục, cũng không phải là mệnh lệnh. Tài liệu dùng “chỉ khi” thay cho “khi và chỉ khi” để một
điều kiện cần một chiều không bị hiểu nhầm thành một quan hệ tương đương. Nhãn
**⚠︎ máy móc** đánh dấu quy tắc có cách diễn đạt tất định, trung thành với công thức
nhưng còn vụng — đó là gợi ý cho một lượt biên tập lại (không bắt buộc) về sau, không phải là lỗi._

| ID | Quy tắc khai báo |
|----|------------------|
| **DR-1 Name** | Trường “name” của mỗi portrait có cùng giá trị với trường “display name” của chính portrait đó. |

## 5 Truy vết đến lược đồ

_Cột biểu thức là định nghĩa của quy tắc theo ký pháp Tài liệu —
cùng một logic mà bộ quy tắc lưu trữ, được viết cho người đọc nghiệp vụ._

| Phần tử lược đồ | Loại | Biểu thức |
|----------------|------|------------|
| **Portraits.Name** | công thức | `DisplayName` |

---

> **RuleSpeak®** — ký pháp quy tắc nghiệp vụ khai báo do **Ronald G. Ross** sáng tạo, theo quy ước **SBVR** — sắp ra mắt. Trong lúc đó, trang này hiển thị tài liệu thông thường. [www.RonRoss.info](https://www.RonRoss.info)
