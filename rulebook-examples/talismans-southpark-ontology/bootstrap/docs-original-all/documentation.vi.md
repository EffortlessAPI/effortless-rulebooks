# 📘 South Park ontology explainer — Tài liệu

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Các quy tắc nghiệp vụ dạng khai báo, được sinh ra từ bộ quy tắc. Mỗi phát biểu
> dưới đây diễn đạt một sự thật trong lĩnh vực nghiệp vụ — không phải là một thủ tục,
> cũng không phải là một mệnh lệnh. Các công thức của bộ quy tắc là nguồn sự thật duy nhất;
> tài liệu này là cách đọc các công thức đó bằng lời thông thường.

## 1 Từ vựng nghiệp vụ

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Có cùng giá trị với trường “graph ID” của chính graph đó. | — |
| Count of Stated Facts | Giá trị tổng cộng của trường “stated unit” trên các statements liên quan đến graph đó. | — |
| Count of Conclusions | Giá trị tổng cộng của trường “conclusion unit” trên các statements liên quan đến graph đó. | _Facts that hold but were never stated._ |
| Count of Violations | Giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến graph đó. | — |
| Is Consistent | Đúng khi count of violations là 0. | — |

<details open>
<summary>Dữ liệu mẫu</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Thuộc tính đã được định nghĩa. | — |
| Explanation | Thuộc tính đã được định nghĩa. | — |
| Is Enabled | Đúng khi chuỗi rỗng. | — |
| Name | Có cùng giá trị với trường “label” của chính rule đó. | — |
| Count of Firings | Giá trị tổng cộng của trường “fired unit” trên các firings liên quan đến rule đó. | _How many times the rule produced a fact._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Thuộc tính đã được định nghĩa. | — |
| Kind | Thuộc tính đã được định nghĩa. | — |
| Name | Có cùng giá trị với trường “label” của chính thing đó. | — |
| Count of Children | Giá trị tổng cộng của trường “parent unit” trên các statements liên quan đến thing đó. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Giá trị tổng cộng của trường “grandparent unit” trên các statements liên quan đến thing đó. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Thuộc tính đã được định nghĩa. | — |
| Inverse of | Thuộc tính đã được định nghĩa. | — |
| Is Symmetric | Đúng khi chuỗi rỗng. | — |
| Chain First | Thuộc tính đã được định nghĩa. | — |
| Chain Second | Thuộc tính đã được định nghĩa. | — |
| Domain Class | Thuộc tính đã được định nghĩa. | — |
| Range Class | Thuộc tính đã được định nghĩa. | — |
| Disjoint With | Thuộc tính đã được định nghĩa. | — |
| Name | Có cùng giá trị với trường “label” của chính property đó. | — |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Thuộc tính đã được định nghĩa. | — |
| Subject | Thuộc tính đã được định nghĩa. | — |
| Property | Thuộc tính đã được định nghĩa. | — |
| Object Thing | Thuộc tính đã được định nghĩa. | — |
| Number Value | Thuộc tính đã được định nghĩa. | — |
| Is Stated | Đúng khi chuỗi rỗng. | _Drawn as a solid line in the explainer._ |
| Subject Label | Lấy từ bản ghi liên kết qua trường “subject”. | — |
| Property Label | Lấy từ bản ghi liên kết qua trường “property”. | — |
| Object Label | Lấy từ bản ghi liên kết qua trường “object thing”. | — |
| Name | Được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là number value nếu object thing để trống, trong mọi trường hợp còn lại là object label. | — |
| Count of Inverse Firings | Giá trị tổng cộng của trường “inverse fired unit” trên các firings liên quan đến statement đó. | — |
| Count of Symmetric Firings | Giá trị tổng cộng của trường “symmetric fired unit” trên các firings liên quan đến statement đó. | — |
| Holds Before Chain | Đúng khi ít nhất một trong các điều kiện sau đúng: cờ “stated” được bật, count of inverse firings lớn hơn 0 hoặc count of symmetric firings lớn hơn 0. | — |
| Count of Chain Firings | Giá trị tổng cộng của trường “chain fired unit” trên các firings liên quan đến statement đó. | — |
| Holds Before Typing | Đúng khi ít nhất một trong các điều kiện sau đúng: cờ “holds before chain” được bật hoặc count of chain firings lớn hơn 0. | — |
| Count of Type Firings | Giá trị tổng cộng của trường “type fired unit” trên các firings liên quan đến statement đó. | — |
| Holds | Đúng khi ít nhất một trong các điều kiện sau đúng: cờ “holds before typing” được bật hoặc count of type firings lớn hơn 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Đúng khi tất cả các điều kiện sau đều đúng: cờ “holds” được bật và cờ “stated” không được bật. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Được xác định theo thứ tự ưu tiên: chuỗi rỗng nếu cờ “conclusion” không được bật; “inverse” nếu count of inverse firings lớn hơn 0; “symmetric” nếu count of symmetric firings lớn hơn 0; “property chain” nếu count of chain firings lớn hơn 0; trong mọi trường hợp còn lại, “domain and range”. | — |
| Stated Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “stated” được bật; trong mọi trường hợp còn lại, 0. | — |
| Conclusion Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “conclusion” được bật; trong mọi trường hợp còn lại, 0. | — |
| Parent Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isParentOf”; trong mọi trường hợp còn lại, 0. | — |
| Grandparent Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isGrandparentOf”; trong mọi trường hợp còn lại, 0. | — |

<details open>
<summary>Dữ liệu mẫu</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Thuộc tính đã được định nghĩa. | — |
| Kind | Thuộc tính đã được định nghĩa. | — |
| Target | Thuộc tính đã được định nghĩa. | — |
| First | Thuộc tính đã được định nghĩa. | — |
| Second | Thuộc tính đã được định nghĩa. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Đúng khi bản ghi liên kết qua trường “rule” là “enabled”. | — |
| Rule Label | Lấy từ bản ghi liên kết qua trường “rule”. | — |
| Target Name | Lấy từ bản ghi liên kết qua trường “target”. | — |
| First Name | Lấy từ bản ghi liên kết qua trường “first”. | — |
| Second Name | Lấy từ bản ghi liên kết qua trường “second”. | — |
| First is Stated | Đúng khi bản ghi liên kết qua trường “first” là “stated”. | — |
| First Holds Before Chain | Đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before chain”. | — |
| Second Holds Before Chain | Đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds before chain”. | — |
| First Holds Before Typing | Đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before typing”. | — |
| Inverse Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “inverse” và cờ “first is stated” được bật; trong mọi trường hợp còn lại, 0. | — |
| Symmetric Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “symmetric” và cờ “first is stated” được bật; trong mọi trường hợp còn lại, 0. | — |
| Chain Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “chain”, cờ “first holds before chain” được bật và cờ “second holds before chain” được bật; trong mọi trường hợp còn lại, 0. | — |
| Type Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật; ít nhất một trong các điều kiện sau đúng: kind là “domain” hoặc kind là “range”; và cờ “first holds before typing” được bật; trong mọi trường hợp còn lại, 0. | — |
| Fired Unit | Được tính bằng inverse fired unit cộng symmetric fired unit cộng chain fired unit cộng type fired unit. | — |
| Name | Được xác định theo thứ tự ưu tiên: target name, tiếp theo là “: worked out by the ”, tiếp theo là rule label, tiếp theo là “ rule from '”, tiếp theo là first name, tiếp theo là “'”, tiếp theo là “ and '”, tiếp theo là second name, tiếp theo là “'” nếu fired unit lớn hơn 0; trong mọi trường hợp còn lại, firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Thuộc tính đã được định nghĩa. | — |
| Rule | Thuộc tính đã được định nghĩa. | — |
| A | Thuộc tính đã được định nghĩa. | — |
| B | Thuộc tính đã được định nghĩa. | — |
| Name | Có cùng giá trị với trường “disjoint check ID” của chính disjoint check đó. | — |
| Rule Enabled | Đúng khi bản ghi liên kết qua trường “rule” là “enabled”. | — |
| A Holds | Đúng khi bản ghi liên kết qua trường “a” thỏa điều kiện “holds”. | — |
| B Holds | Đúng khi bản ghi liên kết qua trường “b” thỏa điều kiện “holds”. | — |
| Violation Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, cờ “a holds” được bật và cờ “b holds” được bật; trong mọi trường hợp còn lại, 0. | — |

<details open>
<summary>Dữ liệu mẫu</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ đánh dấu cột được tính._

</details>

## 2 Kiểu dữ kiện

- Mỗi **property** có thể tham chiếu đến một **property**
- Mỗi **property** có thể tham chiếu đến một **thing**
- Mỗi **statement** tham chiếu đến đúng một **graph**
- Mỗi **statement** tham chiếu đến đúng một **thing**
- Mỗi **statement** tham chiếu đến đúng một **property**
- Mỗi **firing** tham chiếu đến đúng một **rule**
- Mỗi **firing** tham chiếu đến đúng một **statement**
- Mỗi **disjoint check** tham chiếu đến đúng một **graph**
- Mỗi **disjoint check** tham chiếu đến đúng một **rule**
- Mỗi **disjoint check** tham chiếu đến đúng một **statement**

## 3 Quy tắc tác nghiệp

_Quy tắc tác nghiệp nêu những điều mà doanh nghiệp **bắt buộc**, **nghiêm cấm** hoặc
khuyến nghị (**nên**). Quy tắc cấu trúc bắt nguồn từ các trường bắt buộc và khóa ngoại;
quy tắc ngữ nghĩa bắt nguồn từ bảng Constraints, mỗi quy tắc căn cứ vào một giá trị logic mà bộ quy tắc
đã tính sẵn (được tham chiếu chéo dưới dạng DR-N trong phần Quy tắc định nghĩa bên dưới)._

### Ràng buộc cấu trúc (từ lược đồ)

- Mỗi rule **phải** có label và explanation, đồng thời được ghi nhận rõ là “enabled” hay không.
- Mỗi thing **phải** có label và kind.
- Mỗi property **phải** có label, đồng thời được ghi nhận rõ là “symmetric” hay không.
- Mỗi statement **phải** tham chiếu đến đúng một graph.
- Mỗi statement **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi statement **phải** tham chiếu đến đúng một property.
- Mỗi statement **phải** được ghi nhận rõ là “stated” hay không.
- Mỗi firing **phải** tham chiếu đến đúng một rule.
- Mỗi firing **phải** tham chiếu đến đúng một statement với vai trò target.
- Mỗi firing **phải** tham chiếu đến đúng một statement với vai trò first.
- Mỗi firing **phải** tham chiếu đến đúng một statement với vai trò second.
- Mỗi firing **phải** có kind.
- Mỗi disjoint check **phải** tham chiếu đến đúng một graph.
- Mỗi disjoint check **phải** tham chiếu đến đúng một rule.
- Mỗi disjoint check **phải** tham chiếu đến đúng một statement với vai trò a.
- Mỗi disjoint check **phải** tham chiếu đến đúng một statement với vai trò b.

## 4 Quy tắc định nghĩa

_Mọi phát biểu đều diễn đạt sự thật trong lĩnh vực nghiệp vụ; chúng không phải là
thủ tục, cũng không phải là mệnh lệnh. Tài liệu dùng “chỉ khi” thay cho “khi và chỉ khi” để một
điều kiện cần một chiều không bị hiểu nhầm thành một quan hệ tương đương. Nhãn
**⚠︎ máy móc** đánh dấu quy tắc có cách diễn đạt tất định, trung thành với công thức
nhưng còn vụng — đó là gợi ý cho một lượt biên tập lại (không bắt buộc) về sau, không phải là lỗi._

| ID | Quy tắc khai báo |
|----|------------------|
| **DR-1 Name** | Trường “name” của mỗi graph có cùng giá trị với trường “graph ID” của chính graph đó. |
| **DR-2 Count of Stated Facts** | Trường “count of stated facts” của mỗi graph là giá trị tổng cộng của trường “stated unit” trên các statements liên quan đến graph đó. |
| **DR-3 Count of Conclusions** | Trường “count of conclusions” của mỗi graph là giá trị tổng cộng của trường “conclusion unit” trên các statements liên quan đến graph đó. |
| **DR-4 Count of Violations** | Trường “count of violations” của mỗi graph là giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến graph đó. |
| **DR-5 Is Consistent** | Mỗi graph được coi là “consistent” nếu count of violations là 0. |
| **DR-6 Name** | Trường “name” của mỗi rule có cùng giá trị với trường “label” của chính rule đó. |
| **DR-7 Count of Firings** | Trường “count of firings” của mỗi rule là giá trị tổng cộng của trường “fired unit” trên các firings liên quan đến rule đó. |
| **DR-8 Name** | Trường “name” của mỗi thing có cùng giá trị với trường “label” của chính thing đó. |
| **DR-9 Count of Children** | Trường “count of children” của mỗi thing là giá trị tổng cộng của trường “parent unit” trên các statements liên quan đến thing đó. |
| **DR-10 Count of Grandchildren** | Trường “count of grandchildren” của mỗi thing là giá trị tổng cộng của trường “grandparent unit” trên các statements liên quan đến thing đó. |
| **DR-11 Name** | Trường “name” của mỗi property có cùng giá trị với trường “label” của chính property đó. |
| **DR-12 Subject Label** | Trường “subject label” của mỗi statement được lấy từ bản ghi liên kết qua trường “subject”. |
| **DR-13 Property Label** | Trường “property label” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-14 Object Label** | Trường “object label” của mỗi statement được lấy từ bản ghi liên kết qua trường “object thing”. |
| **DR-15 Name** | Trường “name” của mỗi statement được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là number value nếu object thing để trống, trong mọi trường hợp còn lại là object label. |
| **DR-16 Count of Inverse Firings** | Trường “count of inverse firings” của mỗi statement là giá trị tổng cộng của trường “inverse fired unit” trên các firings liên quan đến statement đó. |
| **DR-17 Count of Symmetric Firings** | Trường “count of symmetric firings” của mỗi statement là giá trị tổng cộng của trường “symmetric fired unit” trên các firings liên quan đến statement đó. |
| **DR-18 Holds Before Chain** | Mỗi statement được coi là thỏa điều kiện “holds before chain” nếu ít nhất một trong các điều kiện sau đúng: cờ “stated” được bật, count of inverse firings lớn hơn 0 hoặc count of symmetric firings lớn hơn 0. |
| **DR-19 Count of Chain Firings** | Trường “count of chain firings” của mỗi statement là giá trị tổng cộng của trường “chain fired unit” trên các firings liên quan đến statement đó. |
| **DR-20 Holds Before Typing** | Mỗi statement được coi là thỏa điều kiện “holds before typing” nếu ít nhất một trong các điều kiện sau đúng: cờ “holds before chain” được bật hoặc count of chain firings lớn hơn 0. |
| **DR-21 Count of Type Firings** | Trường “count of type firings” của mỗi statement là giá trị tổng cộng của trường “type fired unit” trên các firings liên quan đến statement đó. |
| **DR-22 Holds** | Mỗi statement được đánh dấu là “holds” nếu ít nhất một trong các điều kiện sau đúng: cờ “holds before typing” được bật hoặc count of type firings lớn hơn 0. |
| **DR-23 Is Conclusion** | Mỗi statement được coi là “conclusion” nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và cờ “stated” không được bật. |
| **DR-24 Worked Out by** | Trường “worked out by” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. chuỗi rỗng, nếu cờ “conclusion” không được bật;<br>2. “inverse”, nếu count of inverse firings lớn hơn 0;<br>3. “symmetric”, nếu count of symmetric firings lớn hơn 0;<br>4. “property chain”, nếu count of chain firings lớn hơn 0;<br>5. trong mọi trường hợp còn lại, “domain and range”. |
| **DR-25 Stated Unit** | Trường “stated unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “stated” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-26 Conclusion Unit** | Trường “conclusion unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “conclusion” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-27 Parent Unit** | Trường “parent unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isParentOf”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-28 Grandparent Unit** | Trường “grandparent unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isGrandparentOf”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-29 Rule Enabled** | Trường “rule enabled” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-30 Rule Label** | Trường “rule label” của mỗi firing được lấy từ bản ghi liên kết qua trường “rule”. |
| **DR-31 Target Name** | Trường “target name” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-32 First Name** | Trường “first name” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-33 Second Name** | Trường “second name” của mỗi firing được lấy từ bản ghi liên kết qua trường “second”. |
| **DR-34 First is Stated** | Trường “first is stated” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” là “stated”. |
| **DR-35 First Holds Before Chain** | Trường “first holds before chain” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before chain”. |
| **DR-36 Second Holds Before Chain** | Trường “second holds before chain” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds before chain”. |
| **DR-37 First Holds Before Typing** | Trường “first holds before typing” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before typing”. |
| **DR-38 Inverse Fired Unit** | Trường “inverse fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “inverse” và cờ “first is stated” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-39 Symmetric Fired Unit** | Trường “symmetric fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “symmetric” và cờ “first is stated” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-40 Chain Fired Unit** | Trường “chain fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “chain”, cờ “first holds before chain” được bật và cờ “second holds before chain” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-41 Type Fired Unit** | Trường “type fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật; ít nhất một trong các điều kiện sau đúng: kind là “domain” hoặc kind là “range”; và cờ “first holds before typing” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-42 Fired Unit** | Trường “fired unit” của mỗi firing được tính bằng inverse fired unit cộng symmetric fired unit cộng chain fired unit cộng type fired unit. |
| **DR-43 Name** | Trường “name” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. target name, tiếp theo là “: worked out by the ”, tiếp theo là rule label, tiếp theo là “ rule from '”, tiếp theo là first name, tiếp theo là “'”, tiếp theo là “ and '”, tiếp theo là second name, tiếp theo là “'”, nếu fired unit lớn hơn 0;<br>2. trong mọi trường hợp còn lại, firing ID. |
| **DR-44 Name** | Trường “name” của mỗi disjoint check có cùng giá trị với trường “disjoint check ID” của chính disjoint check đó. |
| **DR-45 Rule Enabled** | Trường “rule enabled” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-46 A Holds** | Trường “a holds” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “a” thỏa điều kiện “holds”. |
| **DR-47 B Holds** | Trường “b holds” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “b” thỏa điều kiện “holds”. |
| **DR-48 Violation Unit** | Trường “violation unit” của mỗi disjoint check được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, cờ “a holds” được bật và cờ “b holds” được bật;<br>2. trong mọi trường hợp còn lại, 0. |

## 5 Truy vết đến lược đồ

_Cột biểu thức là định nghĩa của quy tắc theo ký pháp Tài liệu —
cùng một logic mà bộ quy tắc lưu trữ, được viết cho người đọc nghiệp vụ._

| Phần tử lược đồ | Loại | Biểu thức |
|----------------|------|------------|
| **Graphs.Name** | công thức | `GraphId` |
| **Graphs.CountOfStatedFacts** | tổng hợp | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | tổng hợp | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | tổng hợp | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | công thức | `CountOfViolations = 0` |
| **Rules.Name** | công thức | `Label` |
| **Rules.CountOfFirings** | tổng hợp | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | công thức | `Label` |
| **Things.CountOfChildren** | tổng hợp | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | tổng hợp | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | công thức | `Label` |
| **Statements.SubjectLabel** | tra cứu | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | tra cứu | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | tra cứu | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | công thức | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | tổng hợp | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | tổng hợp | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | công thức | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | tổng hợp | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | công thức | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | tổng hợp | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | công thức | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | công thức | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | công thức | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | công thức | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | công thức | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | công thức | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | công thức | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | tra cứu | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | tra cứu | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | tra cứu | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | tra cứu | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | tra cứu | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | tra cứu | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | tra cứu | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | tra cứu | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | tra cứu | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | công thức | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | công thức | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | công thức | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | công thức | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | tra cứu | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | tra cứu | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | tra cứu | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | công thức | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
