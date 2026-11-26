# 📘 Talisman's South Park ontology explainer — Tài liệu

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

> Các quy tắc nghiệp vụ dạng khai báo, được sinh ra từ bộ quy tắc. Mỗi phát biểu
> dưới đây diễn đạt một sự thật trong lĩnh vực nghiệp vụ — không phải là một thủ tục,
> cũng không phải là một mệnh lệnh. Các công thức của bộ quy tắc là nguồn sự thật duy nhất;
> tài liệu này là cách đọc các công thức đó bằng lời thông thường.

## 1 Từ vựng nghiệp vụ

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Thuộc tính đã được định nghĩa. | _Display name of the graph._ |
| Literal Quote | Thuộc tính đã được định nghĩa. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Thuộc tính đã được định nghĩa. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Thuộc tính đã được định nghĩa. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Thuộc tính đã được định nghĩa. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Thuộc tính đã được định nghĩa. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Có cùng giá trị với trường “label” của chính graph đó. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Giá trị tổng cộng của trường “in effect unit” trên các statements liên quan đến graph đó. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Giá trị tổng cộng của trường “conclusion unit” trên các statements liên quan đến graph đó. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến graph đó. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Đúng khi count of violations là 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Được xác định theo thứ tự ưu tiên: no conclusions text nếu count of conclusions là 0; trong mọi trường hợp còn lại, chuỗi rỗng. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Giá trị tổng cộng của trường “listed off unit” trên các statements liên quan đến graph đó. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Đúng khi count of facts to add lớn hơn 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Giá trị nhỏ nhất của trường “sort order” trên các statements liên quan đến graph đó, chỉ tính các bản ghi là “listed off unit”. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | Giá trị lớn nhất của trường “activation seq” trên các statements liên quan đến graph đó. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Được tính bằng max activation seq cộng 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Giá trị lớn nhất của trường “sort order” trên các statements liên quan đến graph đó, chỉ tính các bản ghi là “listed unit”. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Được tính bằng max listed sort order cộng 1. | _Order 3. The list position a newly added fact takes._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Thuộc tính đã được định nghĩa. | _The rule's name on the page._ |
| Explanation | Thuộc tính đã được định nghĩa. | _The rule's one-line explanation on the page._ |
| Is Enabled | Đúng khi chuỗi rỗng. | _The rule's checkbox._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the rule list._ |
| Name | Có cùng giá trị với trường “label” của chính rule đó. | _Order 1. The rule's name._ |
| Count of Firings | Giá trị tổng cộng của trường “fired unit” trên các firings liên quan đến rule đó. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
| Rule | Thuộc tính đã được định nghĩa. | _The rule switch this kind belongs to._ |
| Wording | Thuộc tính đã được định nghĩa. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Có cùng giá trị với trường “wording” của chính firing kind đó. | _Order 1. The kind's wording._ |
| Rule Enabled | Đúng khi bản ghi liên kết qua trường “rule” là “enabled”. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Thuộc tính đã được định nghĩa. | _The name the page shows._ |
| Kind | Thuộc tính đã được định nghĩa. | _'individual' or 'class'._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the cast._ |
| Portrait Name | Thuộc tính đã được định nghĩa. | _The name on the portrait's Fandom page._ |
| Portrait Page | Thuộc tính đã được định nghĩa. | _The Fandom page credited for the portrait._ |
| Portrait Image | Thuộc tính đã được định nghĩa. | _The original Fandom image._ |
| Portrait Crop | Thuộc tính đã được định nghĩa. | _The portrait's crop as an SVG viewBox: x y width height._ |
| Name | Có cùng giá trị với trường “label” của chính thing đó. | _Order 1. The thing's name._ |
| Count of Children | Giá trị tổng cộng của trường “parent unit” trên các statements liên quan đến thing đó. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Giá trị tổng cộng của trường “grandparent unit” trên các statements liên quan đến thing đó. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Giá trị tổng cộng của trường “place unit” trên các statements liên quan đến thing đó. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. | _Order 17. Holding facts about this thing._ |
| Count As Object | Giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Đúng khi ít nhất một trong các điều kiện sau đúng: count as subject lớn hơn 0 hoặc count as object lớn hơn 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Được xác định theo thứ tự ưu tiên: “class” nếu kind là “class”; “place” nếu count as place lớn hơn 0; trong mọi trường hợp còn lại, “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | Thuộc tính đã được định nghĩa. | _The property's wording on the page._ |
| Property Kind | Thuộc tính đã được định nghĩa. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Thuộc tính đã được định nghĩa. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Đúng khi chuỗi rỗng. | _Offered in the page's Add fact form._ |
| Has One Current Value | Đúng khi chuỗi rỗng. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | Thuộc tính đã được định nghĩa. | _The property the inverse rule derives from this one._ |
| Is Symmetric | Đúng khi chuỗi rỗng. | _The symmetric rule makes this link true in both directions._ |
| Chain First | Thuộc tính đã được định nghĩa. | _First link of the property chain that derives this property._ |
| Chain Second | Thuộc tính đã được định nghĩa. | _Second link of the property chain that derives this property._ |
| Domain Class | Thuộc tính đã được định nghĩa. | _The class the domain rule gives this property's subject._ |
| Range Class | Thuộc tính đã được định nghĩa. | _The class the range rule gives this property's object._ |
| Disjoint With | Thuộc tính đã được định nghĩa. | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the Add fact form's property list._ |
| Name | Có cùng giá trị với trường “label” của chính property đó. | _Order 1. The property's wording._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | is parent of |
| is child of | object | — | true | false | false | 2 | is child of |
| is spouse of | object | — | true | false | true | 3 | is spouse of |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Thuộc tính đã được định nghĩa. | _The graph the fact belongs to._ |
| Subject | Thuộc tính đã được định nghĩa. | _What the fact describes._ |
| Property | Thuộc tính đã được định nghĩa. | _How the subject connects to the object._ |
| Object Thing | Thuộc tính đã được định nghĩa. | _The related thing, for an object or type property._ |
| Object Value | Thuộc tính đã được định nghĩa. | _The literal value, for a datatype property._ |
| Is Listed | Đúng khi chuỗi rỗng. | _Shown in the page's fact list._ |
| Is Built in | Đúng khi chuỗi rỗng. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Đúng khi chuỗi rỗng. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Đúng khi chuỗi rỗng. | _The fact's checkbox._ |
| Activation Seq | Thuộc tính đã được định nghĩa. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Lấy từ bản ghi liên kết qua trường “subject”. | _Order 1. The subject's name._ |
| Property Label | Lấy từ bản ghi liên kết qua trường “property”. | _Order 1. The property's wording._ |
| Object Label | Lấy từ bản ghi liên kết qua trường “object thing”. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Lấy từ bản ghi liên kết qua trường “property”. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Đúng khi bản ghi liên kết qua trường “property” có “one current value”. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Lấy từ bản ghi liên kết qua trường “property”. | _Order 1. The property's domain class._ |
| Property Range Class | Lấy từ bản ghi liên kết qua trường “property”. | _Order 1. The property's range class._ |
| Literal Quote | Lấy từ bản ghi liên kết qua trường “graph”. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Lấy từ bản ghi liên kết qua trường “graph”. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Lấy từ bản ghi liên kết qua trường “graph”. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Lấy từ bản ghi liên kết qua trường “graph”. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Lấy từ bản ghi liên kết qua trường “graph”. | _Order 6. List position of the next fact to add._ |
| Is Literal | Đúng khi object thing để trống. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Được xác định theo thứ tự ưu tiên: object value nếu object thing để trống; trong mọi trường hợp còn lại, object label. | _Order 2. The object's name or the value._ |
| Name | Được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là object text. | _Order 3. The fact in words._ |
| Words | Được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là literal quote, tiếp theo là object value, tiếp theo là literal quote, tiếp theo là literal type marker, tiếp theo là property datatype nếu object thing để trống, trong mọi trường hợp còn lại là object label. ⚠︎ máy móc <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Được xác định theo thứ tự ưu tiên: built in source text nếu cờ “built in” được bật; trong mọi trường hợp còn lại, user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Được tính bằng subject, tiếp theo là “|”, tiếp theo là property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | Giá trị lớn nhất của trường “activation seq” trên các statements liên quan đến statement đó. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Đúng khi tất cả các điều kiện sau đều đúng: cờ “stated” được bật và ít nhất một trong các điều kiện sau đúng: cờ “property has one current value” không được bật hoặc activation seq là latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “listed” được bật; trong mọi trường hợp còn lại, 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “in effect” được bật; trong mọi trường hợp còn lại, 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “listed” được bật và cờ “in effect” không được bật; trong mọi trường hợp còn lại, 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Đúng khi tất cả các điều kiện sau đều đúng: listed off unit là 1 và sort order là next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Giá trị tổng cộng của trường “inverse fired unit” trên các firings liên quan đến statement đó. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Giá trị tổng cộng của trường “symmetric fired unit” trên các firings liên quan đến statement đó. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Đúng khi ít nhất một trong các điều kiện sau đúng: cờ “in effect” được bật, count of inverse firings lớn hơn 0 hoặc count of symmetric firings lớn hơn 0. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | Giá trị tổng cộng của trường “chain fired unit” trên các firings liên quan đến statement đó. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Đúng khi ít nhất một trong các điều kiện sau đúng: cờ “holds before chain” được bật hoặc count of chain firings lớn hơn 0. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | Giá trị tổng cộng của trường “type fired unit” trên các firings liên quan đến statement đó. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Đúng khi ít nhất một trong các điều kiện sau đúng: cờ “holds before typing” được bật hoặc count of type firings lớn hơn 0. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | Đúng khi tất cả các điều kiện sau đều đúng: cờ “holds” được bật và cờ “in effect” không được bật. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “holds” được bật; trong mọi trường hợp còn lại, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “conclusion” được bật; trong mọi trường hợp còn lại, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isParentOf”; trong mọi trường hợp còn lại, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isGrandparentOf”; trong mọi trường hợp còn lại, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “attends”; trong mọi trường hợp còn lại, 0. | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | Giá trị nhỏ nhất của trường “order key” trên các firings liên quan đến statement đó, chỉ tính các bản ghi là “fired unit”. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Giá trị tổng cộng của trường “chosen unit” trên các firings liên quan đến statement đó. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Thuộc tính đã được định nghĩa. | _How the rule applies._ |
| Rule | Thuộc tính đã được định nghĩa. | _The rule switch._ |
| Target | Thuộc tính đã được định nghĩa. | _The fact this application concludes._ |
| First | Thuộc tính đã được định nghĩa. | _The premise._ |
| Second | Thuộc tính đã được định nghĩa. | _The second premise, for the property chain._ |
| Rule Enabled | Đúng khi bản ghi liên kết qua trường “rule” là “enabled”. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Lấy từ bản ghi liên kết qua trường “kind”. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Lấy từ bản ghi liên kết qua trường “kind”. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Lấy từ bản ghi liên kết qua trường “target”. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Lấy từ bản ghi liên kết qua trường “target”. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Lấy từ bản ghi liên kết qua trường “target”. | _Order 2. The concluded fact's property._ |
| Target Object Text | Lấy từ bản ghi liên kết qua trường “target”. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Đúng khi bản ghi liên kết qua trường “target” là “conclusion”. | _Order 17. Whether the concluded fact is new._ |
| Target First Fired Order Key | Lấy từ bản ghi liên kết qua trường “target”. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Lấy từ bản ghi liên kết qua trường “first”. | _Order 3. The premise as the card quotes it._ |
| Second Words | Lấy từ bản ghi liên kết qua trường “second”. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Đúng khi bản ghi liên kết qua trường “first” là “in effect”. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before chain”. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds before chain”. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before typing”. | _Order 12. Whether the premise holds before typing._ |
| First Property | Lấy từ bản ghi liên kết qua trường “first”. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Lấy từ bản ghi liên kết qua trường “first”. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Lấy từ bản ghi liên kết qua trường “first”. | _Order 2. The premise property's range class._ |
| First Sort Order | Lấy từ bản ghi liên kết qua trường “first”. | _Order 1. The premise's list position._ |
| Second Sort Order | Lấy từ bản ghi liên kết qua trường “second”. | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “inverse” và cờ “first is in effect” được bật; trong mọi trường hợp còn lại, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “symmetric” và cờ “first is in effect” được bật; trong mọi trường hợp còn lại, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “chain”, cờ “first holds before chain” được bật và cờ “second holds before chain” được bật; trong mọi trường hợp còn lại, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật; ít nhất một trong các điều kiện sau đúng: kind là “domain” hoặc kind là “range”; và cờ “first holds before typing” được bật; trong mọi trường hợp còn lại, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Được tính bằng inverse fired unit cộng symmetric fired unit cộng chain fired unit cộng type fired unit. | _Order 14. 1 when this application fires._ |
| Order Key | Được tính bằng first sort order nhân 100000 cộng kind sort order nhân 1000 cộng 0 nếu second để trống, trong mọi trường hợp còn lại là second sort order. | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | Đúng khi tất cả các điều kiện sau đều đúng: fired unit là 1, cờ “target is conclusion” được bật và order key là target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “chosen” được bật; trong mọi trường hợp còn lại, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Được xác định theo thứ tự ưu tiên: “Worked out by the ”, tiếp theo là kind wording, tiếp theo là “ from “”, tiếp theo là first words, tiếp theo là “””, tiếp theo là “ and “”, tiếp theo là second words, tiếp theo là “”” nếu kind là “chain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has domain ”, tiếp theo là first property domain class, tiếp theo là “”” nếu kind là “domain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has range ”, tiếp theo là first property range class, tiếp theo là “””, tiếp theo là dấu chấm nếu cờ “chosen” được bật; trong mọi trường hợp còn lại, chuỗi rỗng. ⚠︎ máy móc <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Có cùng giá trị với trường “firing ID” của chính firing đó. | _Order 1. The application's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Thuộc tính đã được định nghĩa. | _The graph._ |
| Rule | Thuộc tính đã được định nghĩa. | _The rule switch._ |
| First | Thuộc tính đã được định nghĩa. | _The first link._ |
| Second | Thuộc tính đã được định nghĩa. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Đúng khi bản ghi liên kết qua trường “rule” là “enabled”. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds”. | _Order 16. Whether the first link holds._ |
| Second Holds | Đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds”. | _Order 16. Whether the second link holds._ |
| Subject Label | Lấy từ bản ghi liên kết qua trường “first”. | _Order 2. The person recorded with both links._ |
| Object Label | Lấy từ bản ghi liên kết qua trường “first”. | _Order 2. The other person._ |
| Violation Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, cờ “first holds” được bật và cờ “second holds” được bật; trong mọi trường hợp còn lại, 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Được xác định theo thứ tự ưu tiên: “Contradiction found. ”, tiếp theo là subject label, tiếp theo là “ is recorded as both parent and spouse of ”, tiếp theo là object label, tiếp theo là “. Check those facts and the selected disjoint rule.” nếu violation unit là 1; trong mọi trường hợp còn lại, chuỗi rỗng. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Có cùng giá trị với trường “disjoint check ID” của chính disjoint check đó. | _Order 1. The check's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ đánh dấu cột được tính._

</details>

## 2 Kiểu dữ kiện

- Mỗi **firing kind** tham chiếu đến đúng một **rule**
- Mỗi **property** có thể tham chiếu đến một **property**
- Mỗi **property** có thể tham chiếu đến một **thing**
- Mỗi **statement** tham chiếu đến đúng một **graph**
- Mỗi **statement** tham chiếu đến đúng một **thing**
- Mỗi **statement** tham chiếu đến đúng một **property**
- Mỗi **firing** tham chiếu đến đúng một **firing kind**
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

- Mỗi graph **phải** có label, literal quote, literal type marker, built in source text, user source text và no conclusions text.
- Mỗi rule **phải** có label, explanation và sort order, đồng thời được ghi nhận rõ là “enabled” hay không.
- Mỗi firing kind **phải** tham chiếu đến đúng một rule.
- Mỗi firing kind **phải** có wording và sort order.
- Mỗi thing **phải** có label, kind và sort order.
- Mỗi property **phải** có label, property kind và sort order, đồng thời được ghi nhận rõ là “user addable” hay không, có “one current value” hay không và là “symmetric” hay không.
- Mỗi statement **phải** tham chiếu đến đúng một graph.
- Mỗi statement **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi statement **phải** tham chiếu đến đúng một property.
- Mỗi statement **phải** có activation seq và sort order, đồng thời được ghi nhận rõ là “listed” hay không, là “built in” hay không, là “detail” hay không và là “stated” hay không.
- Mỗi firing **phải** tham chiếu đến đúng một firing kind với vai trò kind.
- Mỗi firing **phải** tham chiếu đến đúng một rule.
- Mỗi firing **phải** tham chiếu đến đúng một statement với vai trò target.
- Mỗi firing **phải** tham chiếu đến đúng một statement với vai trò first.
- Mỗi disjoint check **phải** tham chiếu đến đúng một graph.
- Mỗi disjoint check **phải** tham chiếu đến đúng một rule.
- Mỗi disjoint check **phải** tham chiếu đến đúng một statement với vai trò first.
- Mỗi disjoint check **phải** tham chiếu đến đúng một statement với vai trò second.

## 4 Quy tắc định nghĩa

_Mọi phát biểu đều diễn đạt sự thật trong lĩnh vực nghiệp vụ; chúng không phải là
thủ tục, cũng không phải là mệnh lệnh. Tài liệu dùng “chỉ khi” thay cho “khi và chỉ khi” để một
điều kiện cần một chiều không bị hiểu nhầm thành một quan hệ tương đương. Nhãn
**⚠︎ máy móc** đánh dấu quy tắc có cách diễn đạt tất định, trung thành với công thức
nhưng còn vụng — đó là gợi ý cho một lượt biên tập lại (không bắt buộc) về sau, không phải là lỗi._

| ID | Quy tắc khai báo |
|----|------------------|
| **DR-1 Name** | Trường “name” của mỗi graph có cùng giá trị với trường “label” của chính graph đó. |
| **DR-2 Count of Stated Facts** | Trường “count of stated facts” của mỗi graph là giá trị tổng cộng của trường “in effect unit” trên các statements liên quan đến graph đó. |
| **DR-3 Count of Conclusions** | Trường “count of conclusions” của mỗi graph là giá trị tổng cộng của trường “conclusion unit” trên các statements liên quan đến graph đó. |
| **DR-4 Count of Violations** | Trường “count of violations” của mỗi graph là giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến graph đó. |
| **DR-5 Is Consistent** | Mỗi graph được coi là “consistent” nếu count of violations là 0. |
| **DR-6 Conclusions Message** | Trường “conclusions message” của mỗi graph được xác định theo thứ tự ưu tiên sau:<br>1. no conclusions text, nếu count of conclusions là 0;<br>2. trong mọi trường hợp còn lại, chuỗi rỗng. |
| **DR-7 Count of Facts to Add** | Trường “count of facts to add” của mỗi graph là giá trị tổng cộng của trường “listed off unit” trên các statements liên quan đến graph đó. |
| **DR-8 Has Facts to Add** | Mỗi graph được coi là có “facts to add” nếu count of facts to add lớn hơn 0. |
| **DR-9 Next Fact Sort Order** | Trường “next fact sort order” của mỗi graph là giá trị nhỏ nhất của trường “sort order” trên các statements liên quan đến graph đó, chỉ tính các bản ghi là “listed off unit”. |
| **DR-10 Max Activation Seq** | Trường “max activation seq” của mỗi graph là giá trị lớn nhất của trường “activation seq” trên các statements liên quan đến graph đó. |
| **DR-11 Next Activation Seq** | Trường “next activation seq” của mỗi graph được tính bằng max activation seq cộng 1. |
| **DR-12 Max Listed Sort Order** | Trường “max listed sort order” của mỗi graph là giá trị lớn nhất của trường “sort order” trên các statements liên quan đến graph đó, chỉ tính các bản ghi là “listed unit”. |
| **DR-13 Next Listed Sort Order** | Trường “next listed sort order” của mỗi graph được tính bằng max listed sort order cộng 1. |
| **DR-14 Name** | Trường “name” của mỗi rule có cùng giá trị với trường “label” của chính rule đó. |
| **DR-15 Count of Firings** | Trường “count of firings” của mỗi rule là giá trị tổng cộng của trường “fired unit” trên các firings liên quan đến rule đó. |
| **DR-16 Name** | Trường “name” của mỗi firing kind có cùng giá trị với trường “wording” của chính firing kind đó. |
| **DR-17 Rule Enabled** | Trường “rule enabled” của mỗi firing kind có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-18 Name** | Trường “name” của mỗi thing có cùng giá trị với trường “label” của chính thing đó. |
| **DR-19 Count of Children** | Trường “count of children” của mỗi thing là giá trị tổng cộng của trường “parent unit” trên các statements liên quan đến thing đó. |
| **DR-20 Count of Grandchildren** | Trường “count of grandchildren” của mỗi thing là giá trị tổng cộng của trường “grandparent unit” trên các statements liên quan đến thing đó. |
| **DR-21 Count As Place** | Trường “count as place” của mỗi thing là giá trị tổng cộng của trường “place unit” trên các statements liên quan đến thing đó. |
| **DR-22 Count As Subject** | Trường “count as subject” của mỗi thing là giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. |
| **DR-23 Count As Object** | Trường “count as object” của mỗi thing là giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. |
| **DR-24 Is in Graph** | Mỗi thing được coi là “in graph” nếu ít nhất một trong các điều kiện sau đúng: count as subject lớn hơn 0 hoặc count as object lớn hơn 0. |
| **DR-25 Node Kind** | Trường “node kind” của mỗi thing được xác định theo thứ tự ưu tiên sau:<br>1. “class”, nếu kind là “class”;<br>2. “place”, nếu count as place lớn hơn 0;<br>3. trong mọi trường hợp còn lại, “person”. |
| **DR-26 Name** | Trường “name” của mỗi property có cùng giá trị với trường “label” của chính property đó. |
| **DR-27 Subject Label** | Trường “subject label” của mỗi statement được lấy từ bản ghi liên kết qua trường “subject”. |
| **DR-28 Property Label** | Trường “property label” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-29 Object Label** | Trường “object label” của mỗi statement được lấy từ bản ghi liên kết qua trường “object thing”. |
| **DR-30 Property Datatype** | Trường “property datatype” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-31 Property Has One Current Value** | Trường “property has one current value” của mỗi statement có giá trị đúng khi bản ghi liên kết qua trường “property” có “one current value”. |
| **DR-32 Property Domain Class** | Trường “property domain class” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-33 Property Range Class** | Trường “property range class” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-34 Literal Quote** | Trường “literal quote” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-35 Literal Type Marker** | Trường “literal type marker” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-36 Built in Source Text** | Trường “built in source text” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-37 User Source Text** | Trường “user source text” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-38 Next Fact Sort Order** | Trường “next fact sort order” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-39 Is Literal** | Mỗi statement được coi là “literal” nếu object thing để trống. |
| **DR-40 Object Text** | Trường “object text” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. object value, nếu object thing để trống;<br>2. trong mọi trường hợp còn lại, object label. |
| **DR-41 Name** | Trường “name” của mỗi statement được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là object text. |
| **DR-42 Words** | Trường “words” của mỗi statement được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là literal quote, tiếp theo là object value, tiếp theo là literal quote, tiếp theo là literal type marker, tiếp theo là property datatype nếu object thing để trống, trong mọi trường hợp còn lại là object label. ⚠︎ máy móc <!-- rulespeak:reword --> |
| **DR-43 Source** | Trường “source” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. built in source text, nếu cờ “built in” được bật;<br>2. trong mọi trường hợp còn lại, user source text. |
| **DR-44 Subject Property Key** | Trường “subject property key” của mỗi statement được tính bằng subject, tiếp theo là “|”, tiếp theo là property. |
| **DR-45 Latest Sibling Activation Seq** | Trường “latest sibling activation seq” của mỗi statement là giá trị lớn nhất của trường “activation seq” trên các statements liên quan đến statement đó. |
| **DR-46 Is in Effect** | Mỗi statement được coi là “in effect” nếu tất cả các điều kiện sau đều đúng: cờ “stated” được bật và ít nhất một trong các điều kiện sau đúng: cờ “property has one current value” không được bật hoặc activation seq là latest sibling activation seq. |
| **DR-47 Listed Unit** | Trường “listed unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “listed” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-48 In Effect Unit** | Trường “in effect unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “in effect” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-49 Listed Off Unit** | Trường “listed off unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “listed” được bật và cờ “in effect” không được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-50 Is Next to Add** | Mỗi statement được coi là “next to add” nếu tất cả các điều kiện sau đều đúng: listed off unit là 1 và sort order là next fact sort order. |
| **DR-51 Count of Inverse Firings** | Trường “count of inverse firings” của mỗi statement là giá trị tổng cộng của trường “inverse fired unit” trên các firings liên quan đến statement đó. |
| **DR-52 Count of Symmetric Firings** | Trường “count of symmetric firings” của mỗi statement là giá trị tổng cộng của trường “symmetric fired unit” trên các firings liên quan đến statement đó. |
| **DR-53 Holds Before Chain** | Mỗi statement được coi là thỏa điều kiện “holds before chain” nếu ít nhất một trong các điều kiện sau đúng: cờ “in effect” được bật, count of inverse firings lớn hơn 0 hoặc count of symmetric firings lớn hơn 0. |
| **DR-54 Count of Chain Firings** | Trường “count of chain firings” của mỗi statement là giá trị tổng cộng của trường “chain fired unit” trên các firings liên quan đến statement đó. |
| **DR-55 Holds Before Typing** | Mỗi statement được coi là thỏa điều kiện “holds before typing” nếu ít nhất một trong các điều kiện sau đúng: cờ “holds before chain” được bật hoặc count of chain firings lớn hơn 0. |
| **DR-56 Count of Type Firings** | Trường “count of type firings” của mỗi statement là giá trị tổng cộng của trường “type fired unit” trên các firings liên quan đến statement đó. |
| **DR-57 Holds** | Mỗi statement được đánh dấu là “holds” nếu ít nhất một trong các điều kiện sau đúng: cờ “holds before typing” được bật hoặc count of type firings lớn hơn 0. |
| **DR-58 Is Conclusion** | Mỗi statement được coi là “conclusion” nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và cờ “in effect” không được bật. |
| **DR-59 Holds Unit** | Trường “holds unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “holds” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-60 Conclusion Unit** | Trường “conclusion unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “conclusion” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-61 Parent Unit** | Trường “parent unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isParentOf”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-62 Grandparent Unit** | Trường “grandparent unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isGrandparentOf”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-63 Place Unit** | Trường “place unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “attends”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-64 First Fired Order Key** | Trường “first fired order key” của mỗi statement là giá trị nhỏ nhất của trường “order key” trên các firings liên quan đến statement đó, chỉ tính các bản ghi là “fired unit”. |
| **DR-65 Count of Chosen Firings** | Trường “count of chosen firings” của mỗi statement là giá trị tổng cộng của trường “chosen unit” trên các firings liên quan đến statement đó. |
| **DR-66 Rule Enabled** | Trường “rule enabled” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-67 Kind Wording** | Trường “kind wording” của mỗi firing được lấy từ bản ghi liên kết qua trường “kind”. |
| **DR-68 Kind Sort Order** | Trường “kind sort order” của mỗi firing được lấy từ bản ghi liên kết qua trường “kind”. |
| **DR-69 Target Name** | Trường “target name” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-70 Target Subject Label** | Trường “target subject label” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-71 Target Property Label** | Trường “target property label” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-72 Target Object Text** | Trường “target object text” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-73 Target is Conclusion** | Trường “target is conclusion” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “target” là “conclusion”. |
| **DR-74 Target First Fired Order Key** | Trường “target first fired order key” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-75 First Words** | Trường “first words” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-76 Second Words** | Trường “second words” của mỗi firing được lấy từ bản ghi liên kết qua trường “second”. |
| **DR-77 First is in Effect** | Trường “first is in effect” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” là “in effect”. |
| **DR-78 First Holds Before Chain** | Trường “first holds before chain” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before chain”. |
| **DR-79 Second Holds Before Chain** | Trường “second holds before chain” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds before chain”. |
| **DR-80 First Holds Before Typing** | Trường “first holds before typing” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before typing”. |
| **DR-81 First Property** | Trường “first property” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-82 First Property Domain Class** | Trường “first property domain class” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-83 First Property Range Class** | Trường “first property range class” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-84 First Sort Order** | Trường “first sort order” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-85 Second Sort Order** | Trường “second sort order” của mỗi firing được lấy từ bản ghi liên kết qua trường “second”. |
| **DR-86 Inverse Fired Unit** | Trường “inverse fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “inverse” và cờ “first is in effect” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-87 Symmetric Fired Unit** | Trường “symmetric fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “symmetric” và cờ “first is in effect” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-88 Chain Fired Unit** | Trường “chain fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “chain”, cờ “first holds before chain” được bật và cờ “second holds before chain” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-89 Type Fired Unit** | Trường “type fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật; ít nhất một trong các điều kiện sau đúng: kind là “domain” hoặc kind là “range”; và cờ “first holds before typing” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-90 Fired Unit** | Trường “fired unit” của mỗi firing được tính bằng inverse fired unit cộng symmetric fired unit cộng chain fired unit cộng type fired unit. |
| **DR-91 Order Key** | Trường “order key” của mỗi firing được tính bằng first sort order nhân 100000 cộng kind sort order nhân 1000 cộng 0 nếu second để trống, trong mọi trường hợp còn lại là second sort order. |
| **DR-92 Is Chosen** | Mỗi firing được coi là “chosen” nếu tất cả các điều kiện sau đều đúng: fired unit là 1, cờ “target is conclusion” được bật và order key là target first fired order key. |
| **DR-93 Chosen Unit** | Trường “chosen unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “chosen” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-94 Card Text** | Trường “card text” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. “Worked out by the ”, tiếp theo là kind wording, tiếp theo là “ from “”, tiếp theo là first words, tiếp theo là “””, tiếp theo là “ and “”, tiếp theo là second words, tiếp theo là “”” nếu kind là “chain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has domain ”, tiếp theo là first property domain class, tiếp theo là “”” nếu kind là “domain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has range ”, tiếp theo là first property range class, tiếp theo là “””, tiếp theo là dấu chấm, nếu cờ “chosen” được bật;<br>2. trong mọi trường hợp còn lại, chuỗi rỗng. ⚠︎ máy móc <!-- rulespeak:reword --> |
| **DR-95 Name** | Trường “name” của mỗi firing có cùng giá trị với trường “firing ID” của chính firing đó. |
| **DR-96 Rule Enabled** | Trường “rule enabled” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-97 First Holds** | Trường “first holds” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds”. |
| **DR-98 Second Holds** | Trường “second holds” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds”. |
| **DR-99 Subject Label** | Trường “subject label” của mỗi disjoint check được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-100 Object Label** | Trường “object label” của mỗi disjoint check được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-101 Violation Unit** | Trường “violation unit” của mỗi disjoint check được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, cờ “first holds” được bật và cờ “second holds” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-102 Alarm Text** | Trường “alarm text” của mỗi disjoint check được xác định theo thứ tự ưu tiên sau:<br>1. “Contradiction found. ”, tiếp theo là subject label, tiếp theo là “ is recorded as both parent and spouse of ”, tiếp theo là object label, tiếp theo là “. Check those facts and the selected disjoint rule.”, nếu violation unit là 1;<br>2. trong mọi trường hợp còn lại, chuỗi rỗng. |
| **DR-103 Name** | Trường “name” của mỗi disjoint check có cùng giá trị với trường “disjoint check ID” của chính disjoint check đó. |

## 5 Truy vết đến lược đồ

_Cột biểu thức là định nghĩa của quy tắc theo ký pháp Tài liệu —
cùng một logic mà bộ quy tắc lưu trữ, được viết cho người đọc nghiệp vụ._

| Phần tử lược đồ | Loại | Biểu thức |
|----------------|------|------------|
| **Graphs.Name** | công thức | `Label` |
| **Graphs.CountOfStatedFacts** | tổng hợp | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | tổng hợp | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | tổng hợp | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | công thức | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | công thức | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | tổng hợp | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | công thức | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | tổng hợp | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | tổng hợp | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | công thức | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | tổng hợp | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | công thức | `MaxListedSortOrder + 1` |
| **Rules.Name** | công thức | `Label` |
| **Rules.CountOfFirings** | tổng hợp | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | công thức | `Wording` |
| **FiringKinds.RuleEnabled** | tra cứu | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | công thức | `Label` |
| **Things.CountOfChildren** | tổng hợp | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | tổng hợp | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | tổng hợp | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | tổng hợp | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | tổng hợp | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | công thức | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | công thức | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | công thức | `Label` |
| **Statements.SubjectLabel** | tra cứu | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | tra cứu | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | tra cứu | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | tra cứu | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | tra cứu | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | tra cứu | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | tra cứu | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | tra cứu | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | tra cứu | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | tra cứu | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | tra cứu | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | tra cứu | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | công thức | `ObjectThing = ""` |
| **Statements.ObjectText** | công thức | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | công thức | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | công thức | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | công thức | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | công thức | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | tổng hợp | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | công thức | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | công thức | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | công thức | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | công thức | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | công thức | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | tổng hợp | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | tổng hợp | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | công thức | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | tổng hợp | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | công thức | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | tổng hợp | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | công thức | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | công thức | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | công thức | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | công thức | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | công thức | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | công thức | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | công thức | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | tổng hợp | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | tổng hợp | `Sum(Firings.ChosenUnit via Target)` |
| **Firings.RuleEnabled** | tra cứu | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | tra cứu | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | tra cứu | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | tra cứu | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | tra cứu | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | tra cứu | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | tra cứu | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | tra cứu | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetFirstFiredOrderKey** | tra cứu | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | tra cứu | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | tra cứu | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | tra cứu | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | tra cứu | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | tra cứu | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | tra cứu | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | tra cứu | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | tra cứu | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | tra cứu | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | tra cứu | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondSortOrder** | tra cứu | `Lookup(Statements.SortOrder via Second)` |
| **Firings.InverseFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | công thức | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | công thức | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.OrderKey** | công thức | `FirstSortOrder * 100000 + KindSortOrder * 1000 + If(Second = "", 0, SecondSortOrder)` |
| **Firings.IsChosen** | công thức | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | công thức | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | công thức | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | công thức | `FiringId` |
| **DisjointChecks.RuleEnabled** | tra cứu | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | tra cứu | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | tra cứu | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | tra cứu | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | tra cứu | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | công thức | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | công thức | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | công thức | `DisjointCheckId` |

---

> **RuleSpeak®** — ký pháp quy tắc nghiệp vụ khai báo do **Ronald G. Ross** sáng tạo, theo quy ước **SBVR** — sắp ra mắt. Trong lúc đó, trang này hiển thị tài liệu thông thường. [www.RonRoss.info](https://www.RonRoss.info)
