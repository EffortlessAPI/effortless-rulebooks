# 📘 Talisman's South Park ontology explainer — Tài liệu

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

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
| Max Sort Order | Giá trị lớn nhất của trường “sort order” trên các statements liên quan đến graph đó. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Được tính bằng max sort order cộng 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 | 100269 | 100270 |

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
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
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
| Portrait | Thuộc tính đã được định nghĩa. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | Trường “display name” của bản ghi liên kết qua trường “portrait”. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Lấy từ bản ghi liên kết qua trường “portrait”. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Lấy từ bản ghi liên kết qua trường “portrait”. | _Order 1. The original Fandom image._ |
| Portrait Crop | Lấy từ bản ghi liên kết qua trường “portrait”. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Lấy từ bản ghi liên kết qua trường “portrait”. | _Order 1. The original image's width in pixels._ |
| Portrait Height | Lấy từ bản ghi liên kết qua trường “portrait”. | _Order 1. The original image's height in pixels._ |
| Name | Có cùng giá trị với trường “label” của chính thing đó. | _Order 1. The thing's name._ |
| Node Key | Có cùng giá trị với trường “thing ID” của chính thing đó. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Giá trị tổng cộng của trường “parent unit” trên các statements liên quan đến thing đó. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Giá trị tổng cộng của trường “grandparent unit” trên các statements liên quan đến thing đó. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Giá trị tổng cộng của trường “place unit” trên các statements liên quan đến thing đó. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. | _Order 17. Holding facts about this thing._ |
| Count As Object | Giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Đúng khi ít nhất một trong các điều kiện sau đúng: count as subject lớn hơn 0 hoặc count as object lớn hơn 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Được xác định theo thứ tự ưu tiên: chuỗi rỗng nếu kind để trống; “class” nếu kind là “class”; “place” nếu count as place lớn hơn 0; trong mọi trường hợp còn lại, “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ đánh dấu cột được tính._

</details>

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
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Thuộc tính đã được định nghĩa. | _The portrait this name selects._ |
| Name | Có cùng giá trị với trường “portrait alias ID” của chính portrait aliase đó. | _Order 1. The alias._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Thuộc tính đã được định nghĩa. | _The property's wording on the page._ |
| Property Kind | Thuộc tính đã được định nghĩa. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Thuộc tính đã được định nghĩa. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Đúng khi chuỗi rỗng. | _Offered in the page's Add fact form._ |
| Has One Current Value | Đúng khi chuỗi rỗng. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Đúng khi chuỗi rỗng. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Thuộc tính đã được định nghĩa. | _The class the domain rule gives this property's subject._ |
| Range Class | Thuộc tính đã được định nghĩa. | _The class the range rule gives this property's object._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the Add fact form's property list._ |
| Value Node Label | Thuộc tính đã được định nghĩa. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Thuộc tính đã được định nghĩa. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Thuộc tính đã được định nghĩa. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Thuộc tính đã được định nghĩa. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Thuộc tính đã được định nghĩa. | _Position of this property in the current-record summary._ |
| Name | Có cùng giá trị với trường “label” của chính property đó. | _Order 1. The property's wording._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Thuộc tính đã được định nghĩa. | _inverse, chain or disjoint._ |
| Property | Thuộc tính đã được định nghĩa. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Thuộc tính đã được định nghĩa. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Thuộc tính đã được định nghĩa. | _The chain's second link._ |
| Name | Có cùng giá trị với trường “property axiom ID” của chính property axiom đó. | _Order 1. The axiom's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

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
| Holds Before Chain | Đúng khi kết quả của hàm IF trên kết quả của hàm OR trên is in effect, count of inverse firings lớn hơn 0 và count of symmetric firings lớn hơn 0 và đúng. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | Giá trị tổng cộng của trường “chain fired unit” trên các firings liên quan đến statement đó. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Đúng khi kết quả của hàm IF trên kết quả của hàm OR trên holds before chain và count of chain firings lớn hơn 0 và đúng. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | Giá trị tổng cộng của trường “type fired unit” trên các firings liên quan đến statement đó. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Đúng khi kết quả của hàm IF trên kết quả của hàm OR trên holds before typing và count of type firings lớn hơn 0 và đúng. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | Đúng khi kết quả của hàm IF trên kết quả của hàm AND trên holds và cờ “in effect” không được bật và đúng. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “holds” được bật; trong mọi trường hợp còn lại, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “conclusion” được bật; trong mọi trường hợp còn lại, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isParentOf”; trong mọi trường hợp còn lại, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isGrandparentOf”; trong mọi trường hợp còn lại, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “attends”; trong mọi trường hợp còn lại, 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | Giá trị nhỏ nhất của trường “pre chain order key” trên các firings liên quan đến statement đó, chỉ tính các bản ghi là “pre chain fired unit”. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Được xác định theo thứ tự ưu tiên: sort order nếu cờ “in effect” được bật; 100000 cộng first pre chain order key nếu cờ “holds before chain” được bật; trong mọi trường hợp còn lại, 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | Giá trị nhỏ nhất của trường “order key” trên các firings liên quan đến statement đó, chỉ tính các bản ghi là “fired unit”. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Giá trị tổng cộng của trường “chosen unit” trên các firings liên quan đến statement đó. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Có cùng giá trị với trường “subject” của chính statement đó. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Được xác định theo thứ tự ưu tiên: “literal|”, tiếp theo là object value, tiếp theo là “|”, tiếp theo là property datatype nếu object thing để trống; trong mọi trường hợp còn lại, object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Lấy từ bản ghi liên kết qua trường “subject”. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Lấy từ bản ghi liên kết qua trường “object thing”. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Được xác định theo thứ tự ưu tiên: “value” nếu object thing để trống; trong mọi trường hợp còn lại, object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Lấy từ bản ghi liên kết qua trường “property”. | _Order 1. The small label under a value node._ |
| Count of Violations As First | Giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến statement đó. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến statement đó. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Đúng khi ít nhất một trong các điều kiện sau đúng: count of violations as first lớn hơn 0 hoặc count of violations as second lớn hơn 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | — | 1.0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

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
| Target Subject Node Key | Lấy từ bản ghi liên kết qua trường “target”. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Lấy từ bản ghi liên kết qua trường “target”. | _Order 3. The concluded fact's object node._ |
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
| Second is in Effect | Đúng khi bản ghi liên kết qua trường “second” là “in effect”. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Lấy từ bản ghi liên kết qua trường “first”. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Lấy từ bản ghi liên kết qua trường “second”. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “inverse” và cờ “first is in effect” được bật; trong mọi trường hợp còn lại, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “symmetric” và cờ “first is in effect” được bật; trong mọi trường hợp còn lại, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “chain”, cờ “first holds before chain” được bật và cờ “second holds before chain” được bật; trong mọi trường hợp còn lại, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Được xác định theo thứ tự ưu tiên: 1 nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật; ít nhất một trong các điều kiện sau đúng: kind là “domain” hoặc kind là “range”; và cờ “first holds before typing” được bật; trong mọi trường hợp còn lại, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Được tính bằng inverse fired unit cộng symmetric fired unit cộng chain fired unit cộng type fired unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Được tính bằng inverse fired unit cộng symmetric fired unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Được tính bằng first sort order nhân 10 cộng kind sort order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Được xác định theo thứ tự ưu tiên: 2 nếu tất cả các điều kiện sau đều đúng: kind là “chain” và không xảy ra việc tất cả các điều kiện sau đều đúng: cờ “first is in effect” được bật và cờ “second is in effect” được bật; trong mọi trường hợp còn lại, 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Được tính bằng round nhân 1000000000000000 cộng first snap position nhân 100000000 cộng kind sort order nhân 10000000 cộng 0 nếu second để trống, trong mọi trường hợp còn lại là second snap position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Đúng khi tất cả các điều kiện sau đều đúng: fired unit là 1, cờ “target is conclusion” được bật và order key là target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Được xác định theo thứ tự ưu tiên: 1 nếu cờ “chosen” được bật; trong mọi trường hợp còn lại, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Được xác định theo thứ tự ưu tiên: “Worked out by the ”, tiếp theo là kind wording, tiếp theo là “ from “”, tiếp theo là first words, tiếp theo là “””, tiếp theo là “ and “”, tiếp theo là second words, tiếp theo là “”” nếu kind là “chain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has domain ”, tiếp theo là first property domain class, tiếp theo là “”” nếu kind là “domain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has range ”, tiếp theo là first property range class, tiếp theo là “””, tiếp theo là dấu chấm nếu cờ “chosen” được bật; trong mọi trường hợp còn lại, chuỗi rỗng. ⚠︎ máy móc <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Có cùng giá trị với trường “firing ID” của chính firing đó. | _Order 1. The application's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | — | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

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
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Thuộc tính đã được định nghĩa. | _The graph._ |
| Label | Thuộc tính đã được định nghĩa. | _The button's text._ |
| Note | Thuộc tính đã được định nghĩa. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position among the buttons._ |
| Button Group | Thuộc tính đã được định nghĩa. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Đúng khi chuỗi rỗng. | _Drawn as the filled button._ |
| Fact Baseline | Thuộc tính đã được định nghĩa. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Thuộc tính đã được định nghĩa. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Đúng khi chuỗi rỗng. | _Clears the selected node or line._ |
| Clears Form Message | Đúng khi chuỗi rỗng. | _Clears the Add fact form's message._ |
| Name | Có cùng giá trị với trường “label” của chính scenario đó. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Đúng khi bản ghi liên kết qua trường “graph” có “facts to add”. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Đúng khi ít nhất một trong các điều kiện sau đúng: fact baseline khác “next” hoặc cờ “graph has facts to add” được bật. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Thuộc tính đã được định nghĩa. | _The button._ |
| Subject | Thuộc tính đã được định nghĩa. | _The subject whose facts change._ |
| Property | Thuộc tính đã được định nghĩa. | _The property whose facts change._ |
| Object Key | Thuộc tính đã được định nghĩa. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Thuộc tính đã được định nghĩa. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Đúng khi chuỗi rỗng. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Order of the change within the button._ |
| Name | Có cùng giá trị với trường “scenario fact change ID” của chính scenario fact change đó. | _Order 1. The change's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Thuộc tính đã được định nghĩa. | _The button._ |
| Rule | Thuộc tính đã được định nghĩa. | _The rule switch._ |
| Is Enabled | Đúng khi chuỗi rỗng. | _The state the button puts the switch in._ |
| Name | Có cùng giá trị với trường “scenario rule setting ID” của chính scenario rule setting đó. | _Order 1. The setting's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Thuộc tính đã được định nghĩa. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Thuộc tính đã được định nghĩa. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Thuộc tính đã được định nghĩa. | _Horizontal position._ |
| Y | Thuộc tính đã được định nghĩa. | _Vertical position._ |
| Name | Có cùng giá trị với trường “node layout ID” của chính node layout đó. | _Order 1. The position's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Thuộc tính đã được định nghĩa. | _Order in which slots are used._ |
| X | Thuộc tính đã được định nghĩa. | _Horizontal position._ |
| Y | Thuộc tính đã được định nghĩa. | _Vertical position._ |
| Name | Có cùng giá trị với trường “layout slot ID” của chính layout slot đó. | _Order 1. The slot's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Thuộc tính đã được định nghĩa. | _The string._ |
| Name | Có cùng giá trị với trường “page text ID” của chính page text đó. | _Order 1. The string's key._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Thuộc tính đã được định nghĩa. | _The engine's name in the switcher._ |
| Substrate | Thuộc tính đã được định nghĩa. | _The conformance harness's name for this substrate._ |
| Is Default | Đúng khi chuỗi rỗng. | _The engine a new visitor starts on._ |
| Description | Thuộc tính đã được định nghĩa. | _What computes the page on this engine._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the switcher._ |
| Name | Có cùng giá trị với trường “label” của chính engine đó. | _Order 1. The engine's name._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ đánh dấu cột được tính._

</details>

| Thuật ngữ | Mô tả | Ghi chú diễn giải |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds, for each form of the rulebook. The explainer page and the ontology form's editor list the ontology form's tools; the concept form's editor lists the concept form's. The two forms are the same facts, so they list the same tools. | — |
| Label | Thuộc tính đã được định nghĩa. | _The tool's name._ |
| Adds | Thuộc tính đã được định nghĩa. | _What it adds, for this form of the rulebook._ |
| Href | Thuộc tính đã được định nghĩa. | _Where its link goes; 'editor' means this form's running rulebook editor._ |
| Link Text | Thuộc tính đã được định nghĩa. | _The link's text._ |
| Sort Order | Thuộc tính đã được định nghĩa. | _Position in the panel._ |
| Form | Thuộc tính đã được định nghĩa. | _Which form of the rulebook the tool is about: ontology (things, properties, statements) or concept (tables)._ |
| Name | Có cùng giá trị với trường “label” của chính rulebook tool đó. | _Order 1. The tool's name._ |

<details open>
<summary>Dữ liệu mẫu</summary>

| Label | Adds | Href | Link Text | Sort Order | Form | Name ƒ |
|---|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | ontology | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | ontology | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | ontology | Explainer |

_ƒ đánh dấu cột được tính._

</details>

## 2 Kiểu dữ kiện

- Mỗi **firing kind** tham chiếu đến đúng một **rule**
- Mỗi **thing** có thể tham chiếu đến một **portrait**
- Mỗi **portrait aliase** tham chiếu đến đúng một **portrait**
- Mỗi **property** có thể tham chiếu đến một **thing**
- Mỗi **property axiom** tham chiếu đến đúng một **firing kind**
- Mỗi **property axiom** tham chiếu đến đúng một **property**
- Mỗi **statement** tham chiếu đến đúng một **graph**
- Mỗi **statement** tham chiếu đến đúng một **thing**
- Mỗi **statement** tham chiếu đến đúng một **property**
- Mỗi **firing** tham chiếu đến đúng một **firing kind**
- Mỗi **firing** tham chiếu đến đúng một **rule**
- Mỗi **firing** tham chiếu đến đúng một **statement**
- Mỗi **disjoint check** tham chiếu đến đúng một **graph**
- Mỗi **disjoint check** tham chiếu đến đúng một **rule**
- Mỗi **disjoint check** tham chiếu đến đúng một **statement**
- Mỗi **scenario** tham chiếu đến đúng một **graph**
- Mỗi **scenario fact change** tham chiếu đến đúng một **scenario**
- Mỗi **scenario fact change** tham chiếu đến đúng một **thing**
- Mỗi **scenario fact change** tham chiếu đến đúng một **property**
- Mỗi **scenario rule setting** tham chiếu đến đúng một **scenario**
- Mỗi **scenario rule setting** tham chiếu đến đúng một **rule**

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
- Mỗi portrait **phải** có display name, page, image, crop, width, height và sort order.
- Mỗi portrait aliase **phải** tham chiếu đến đúng một portrait.
- Mỗi property **phải** có label, property kind và sort order, đồng thời được ghi nhận rõ là “user addable” hay không, có “one current value” hay không và là “symmetric” hay không.
- Mỗi property axiom **phải** tham chiếu đến đúng một firing kind với vai trò kind.
- Mỗi property axiom **phải** tham chiếu đến đúng một property.
- Mỗi property axiom **phải** tham chiếu đến đúng một property với vai trò first.
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
- Mỗi scenario **phải** tham chiếu đến đúng một graph.
- Mỗi scenario **phải** có label, sort order, button group và fact baseline, đồng thời được ghi nhận rõ là “primary” hay không, thỏa điều kiện “clears selection” hay không và thỏa điều kiện “clears form message” hay không.
- Mỗi scenario fact change **phải** tham chiếu đến đúng một scenario.
- Mỗi scenario fact change **phải** tham chiếu đến đúng một thing với vai trò subject.
- Mỗi scenario fact change **phải** tham chiếu đến đúng một property.
- Mỗi scenario fact change **phải** có action và sort order, đồng thời được ghi nhận rõ thỏa điều kiện “adds to list” hay không.
- Mỗi scenario rule setting **phải** tham chiếu đến đúng một scenario.
- Mỗi scenario rule setting **phải** tham chiếu đến đúng một rule.
- Mỗi scenario rule setting **phải** được ghi nhận rõ là “enabled” hay không.
- Mỗi node layout **phải** có layout, node key, x và y.
- Mỗi layout slot **phải** có sort order, x và y.
- Mỗi page text **phải** có text.
- Mỗi engine **phải** có label, substrate, description và sort order, đồng thời được ghi nhận rõ là “default” hay không.
- Mỗi rulebook tool **phải** có label, adds, sort order và form.

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
| **DR-14 Max Sort Order** | Trường “max sort order” của mỗi graph là giá trị lớn nhất của trường “sort order” trên các statements liên quan đến graph đó. |
| **DR-15 Next Candidate Sort Order** | Trường “next candidate sort order” của mỗi graph được tính bằng max sort order cộng 1. |
| **DR-16 Name** | Trường “name” của mỗi rule có cùng giá trị với trường “label” của chính rule đó. |
| **DR-17 Count of Firings** | Trường “count of firings” của mỗi rule là giá trị tổng cộng của trường “fired unit” trên các firings liên quan đến rule đó. |
| **DR-18 Name** | Trường “name” của mỗi firing kind có cùng giá trị với trường “wording” của chính firing kind đó. |
| **DR-19 Rule Enabled** | Trường “rule enabled” của mỗi firing kind có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-20 Portrait Name** | Trường “portrait name” của mỗi thing là trường “display name” của bản ghi liên kết qua trường “portrait”. |
| **DR-21 Portrait Page** | Trường “portrait page” của mỗi thing được lấy từ bản ghi liên kết qua trường “portrait”. |
| **DR-22 Portrait Image** | Trường “portrait image” của mỗi thing được lấy từ bản ghi liên kết qua trường “portrait”. |
| **DR-23 Portrait Crop** | Trường “portrait crop” của mỗi thing được lấy từ bản ghi liên kết qua trường “portrait”. |
| **DR-24 Portrait Width** | Trường “portrait width” của mỗi thing được lấy từ bản ghi liên kết qua trường “portrait”. |
| **DR-25 Portrait Height** | Trường “portrait height” của mỗi thing được lấy từ bản ghi liên kết qua trường “portrait”. |
| **DR-26 Name** | Trường “name” của mỗi thing có cùng giá trị với trường “label” của chính thing đó. |
| **DR-27 Node Key** | Trường “node key” của mỗi thing có cùng giá trị với trường “thing ID” của chính thing đó. |
| **DR-28 Count of Children** | Trường “count of children” của mỗi thing là giá trị tổng cộng của trường “parent unit” trên các statements liên quan đến thing đó. |
| **DR-29 Count of Grandchildren** | Trường “count of grandchildren” của mỗi thing là giá trị tổng cộng của trường “grandparent unit” trên các statements liên quan đến thing đó. |
| **DR-30 Count As Place** | Trường “count as place” của mỗi thing là giá trị tổng cộng của trường “place unit” trên các statements liên quan đến thing đó. |
| **DR-31 Count As Subject** | Trường “count as subject” của mỗi thing là giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. |
| **DR-32 Count As Object** | Trường “count as object” của mỗi thing là giá trị tổng cộng của trường “holds unit” trên các statements liên quan đến thing đó. |
| **DR-33 Is in Graph** | Mỗi thing được coi là “in graph” nếu ít nhất một trong các điều kiện sau đúng: count as subject lớn hơn 0 hoặc count as object lớn hơn 0. |
| **DR-34 Node Kind** | Trường “node kind” của mỗi thing được xác định theo thứ tự ưu tiên sau:<br>1. chuỗi rỗng, nếu kind để trống;<br>2. “class”, nếu kind là “class”;<br>3. “place”, nếu count as place lớn hơn 0;<br>4. trong mọi trường hợp còn lại, “person”. |
| **DR-35 Name** | Trường “name” của mỗi portrait có cùng giá trị với trường “display name” của chính portrait đó. |
| **DR-36 Name** | Trường “name” của mỗi portrait aliase có cùng giá trị với trường “portrait alias ID” của chính portrait aliase đó. |
| **DR-37 Name** | Trường “name” của mỗi property có cùng giá trị với trường “label” của chính property đó. |
| **DR-38 Name** | Trường “name” của mỗi property axiom có cùng giá trị với trường “property axiom ID” của chính property axiom đó. |
| **DR-39 Subject Label** | Trường “subject label” của mỗi statement được lấy từ bản ghi liên kết qua trường “subject”. |
| **DR-40 Property Label** | Trường “property label” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-41 Object Label** | Trường “object label” của mỗi statement được lấy từ bản ghi liên kết qua trường “object thing”. |
| **DR-42 Property Datatype** | Trường “property datatype” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-43 Property Has One Current Value** | Trường “property has one current value” của mỗi statement có giá trị đúng khi bản ghi liên kết qua trường “property” có “one current value”. |
| **DR-44 Property Domain Class** | Trường “property domain class” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-45 Property Range Class** | Trường “property range class” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-46 Literal Quote** | Trường “literal quote” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-47 Literal Type Marker** | Trường “literal type marker” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-48 Built in Source Text** | Trường “built in source text” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-49 User Source Text** | Trường “user source text” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-50 Next Fact Sort Order** | Trường “next fact sort order” của mỗi statement được lấy từ bản ghi liên kết qua trường “graph”. |
| **DR-51 Is Literal** | Mỗi statement được coi là “literal” nếu object thing để trống. |
| **DR-52 Object Text** | Trường “object text” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. object value, nếu object thing để trống;<br>2. trong mọi trường hợp còn lại, object label. |
| **DR-53 Name** | Trường “name” của mỗi statement được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là object text. |
| **DR-54 Words** | Trường “words” của mỗi statement được tính bằng subject label, tiếp theo là dấu cách, tiếp theo là property label, tiếp theo là dấu cách, tiếp theo là literal quote, tiếp theo là object value, tiếp theo là literal quote, tiếp theo là literal type marker, tiếp theo là property datatype nếu object thing để trống, trong mọi trường hợp còn lại là object label. ⚠︎ máy móc <!-- rulespeak:reword --> |
| **DR-55 Source** | Trường “source” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. built in source text, nếu cờ “built in” được bật;<br>2. trong mọi trường hợp còn lại, user source text. |
| **DR-56 Subject Property Key** | Trường “subject property key” của mỗi statement được tính bằng subject, tiếp theo là “|”, tiếp theo là property. |
| **DR-57 Latest Sibling Activation Seq** | Trường “latest sibling activation seq” của mỗi statement là giá trị lớn nhất của trường “activation seq” trên các statements liên quan đến statement đó. |
| **DR-58 Is in Effect** | Mỗi statement được coi là “in effect” nếu tất cả các điều kiện sau đều đúng: cờ “stated” được bật và ít nhất một trong các điều kiện sau đúng: cờ “property has one current value” không được bật hoặc activation seq là latest sibling activation seq. |
| **DR-59 Listed Unit** | Trường “listed unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “listed” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-60 In Effect Unit** | Trường “in effect unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “in effect” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-61 Listed Off Unit** | Trường “listed off unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “listed” được bật và cờ “in effect” không được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-62 Is Next to Add** | Mỗi statement được coi là “next to add” nếu tất cả các điều kiện sau đều đúng: listed off unit là 1 và sort order là next fact sort order. |
| **DR-63 Count of Inverse Firings** | Trường “count of inverse firings” của mỗi statement là giá trị tổng cộng của trường “inverse fired unit” trên các firings liên quan đến statement đó. |
| **DR-64 Count of Symmetric Firings** | Trường “count of symmetric firings” của mỗi statement là giá trị tổng cộng của trường “symmetric fired unit” trên các firings liên quan đến statement đó. |
| **DR-65 Holds Before Chain** | Mỗi statement được coi là thỏa điều kiện “holds before chain” nếu kết quả của hàm IF trên kết quả của hàm OR trên is in effect, count of inverse firings lớn hơn 0 và count of symmetric firings lớn hơn 0 và đúng. |
| **DR-66 Count of Chain Firings** | Trường “count of chain firings” của mỗi statement là giá trị tổng cộng của trường “chain fired unit” trên các firings liên quan đến statement đó. |
| **DR-67 Holds Before Typing** | Mỗi statement được coi là thỏa điều kiện “holds before typing” nếu kết quả của hàm IF trên kết quả của hàm OR trên holds before chain và count of chain firings lớn hơn 0 và đúng. |
| **DR-68 Count of Type Firings** | Trường “count of type firings” của mỗi statement là giá trị tổng cộng của trường “type fired unit” trên các firings liên quan đến statement đó. |
| **DR-69 Holds** | Mỗi statement được đánh dấu là “holds” nếu kết quả của hàm IF trên kết quả của hàm OR trên holds before typing và count of type firings lớn hơn 0 và đúng. |
| **DR-70 Is Conclusion** | Mỗi statement được coi là “conclusion” nếu kết quả của hàm IF trên kết quả của hàm AND trên holds và cờ “in effect” không được bật và đúng. |
| **DR-71 Holds Unit** | Trường “holds unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “holds” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-72 Conclusion Unit** | Trường “conclusion unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “conclusion” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-73 Parent Unit** | Trường “parent unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isParentOf”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-74 Grandparent Unit** | Trường “grandparent unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “isGrandparentOf”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-75 Place Unit** | Trường “place unit” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “holds” được bật và property là “attends”;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-76 First Pre Chain Order Key** | Trường “first pre chain order key” của mỗi statement là giá trị nhỏ nhất của trường “pre chain order key” trên các firings liên quan đến statement đó, chỉ tính các bản ghi là “pre chain fired unit”. |
| **DR-77 Snap Position** | Trường “snap position” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. sort order, nếu cờ “in effect” được bật;<br>2. 100000 cộng first pre chain order key, nếu cờ “holds before chain” được bật;<br>3. trong mọi trường hợp còn lại, 0. |
| **DR-78 First Fired Order Key** | Trường “first fired order key” của mỗi statement là giá trị nhỏ nhất của trường “order key” trên các firings liên quan đến statement đó, chỉ tính các bản ghi là “fired unit”. |
| **DR-79 Count of Chosen Firings** | Trường “count of chosen firings” của mỗi statement là giá trị tổng cộng của trường “chosen unit” trên các firings liên quan đến statement đó. |
| **DR-80 Subject Node Key** | Trường “subject node key” của mỗi statement có cùng giá trị với trường “subject” của chính statement đó. |
| **DR-81 Object Node Key** | Trường “object node key” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. “literal|”, tiếp theo là object value, tiếp theo là “|”, tiếp theo là property datatype, nếu object thing để trống;<br>2. trong mọi trường hợp còn lại, object thing. |
| **DR-82 Subject Node Kind** | Trường “subject node kind” của mỗi statement được lấy từ bản ghi liên kết qua trường “subject”. |
| **DR-83 Object Thing Node Kind** | Trường “object thing node kind” của mỗi statement được lấy từ bản ghi liên kết qua trường “object thing”. |
| **DR-84 Object Node Kind** | Trường “object node kind” của mỗi statement được xác định theo thứ tự ưu tiên sau:<br>1. “value”, nếu object thing để trống;<br>2. trong mọi trường hợp còn lại, object thing node kind. |
| **DR-85 Object Value Node Label** | Trường “object value node label” của mỗi statement được lấy từ bản ghi liên kết qua trường “property”. |
| **DR-86 Count of Violations As First** | Trường “count of violations as first” của mỗi statement là giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến statement đó. |
| **DR-87 Count of Violations As Second** | Trường “count of violations as second” của mỗi statement là giá trị tổng cộng của trường “violation unit” trên các disjoint checks liên quan đến statement đó. |
| **DR-88 Is in Contradiction** | Mỗi statement được coi là “in contradiction” nếu ít nhất một trong các điều kiện sau đúng: count of violations as first lớn hơn 0 hoặc count of violations as second lớn hơn 0. |
| **DR-89 Rule Enabled** | Trường “rule enabled” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-90 Kind Wording** | Trường “kind wording” của mỗi firing được lấy từ bản ghi liên kết qua trường “kind”. |
| **DR-91 Kind Sort Order** | Trường “kind sort order” của mỗi firing được lấy từ bản ghi liên kết qua trường “kind”. |
| **DR-92 Target Name** | Trường “target name” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-93 Target Subject Label** | Trường “target subject label” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-94 Target Property Label** | Trường “target property label” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-95 Target Object Text** | Trường “target object text” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-96 Target is Conclusion** | Trường “target is conclusion” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “target” là “conclusion”. |
| **DR-97 Target Subject Node Key** | Trường “target subject node key” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-98 Target Object Node Key** | Trường “target object node key” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-99 Target First Fired Order Key** | Trường “target first fired order key” của mỗi firing được lấy từ bản ghi liên kết qua trường “target”. |
| **DR-100 First Words** | Trường “first words” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-101 Second Words** | Trường “second words” của mỗi firing được lấy từ bản ghi liên kết qua trường “second”. |
| **DR-102 First is in Effect** | Trường “first is in effect” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” là “in effect”. |
| **DR-103 First Holds Before Chain** | Trường “first holds before chain” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before chain”. |
| **DR-104 Second Holds Before Chain** | Trường “second holds before chain” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds before chain”. |
| **DR-105 First Holds Before Typing** | Trường “first holds before typing” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds before typing”. |
| **DR-106 First Property** | Trường “first property” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-107 First Property Domain Class** | Trường “first property domain class” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-108 First Property Range Class** | Trường “first property range class” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-109 First Sort Order** | Trường “first sort order” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-110 Second is in Effect** | Trường “second is in effect” của mỗi firing có giá trị đúng khi bản ghi liên kết qua trường “second” là “in effect”. |
| **DR-111 First Snap Position** | Trường “first snap position” của mỗi firing được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-112 Second Snap Position** | Trường “second snap position” của mỗi firing được lấy từ bản ghi liên kết qua trường “second”. |
| **DR-113 Inverse Fired Unit** | Trường “inverse fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “inverse” và cờ “first is in effect” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-114 Symmetric Fired Unit** | Trường “symmetric fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “symmetric” và cờ “first is in effect” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-115 Chain Fired Unit** | Trường “chain fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, kind là “chain”, cờ “first holds before chain” được bật và cờ “second holds before chain” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-116 Type Fired Unit** | Trường “type fired unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật; ít nhất một trong các điều kiện sau đúng: kind là “domain” hoặc kind là “range”; và cờ “first holds before typing” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-117 Fired Unit** | Trường “fired unit” của mỗi firing được tính bằng inverse fired unit cộng symmetric fired unit cộng chain fired unit cộng type fired unit. |
| **DR-118 Pre Chain Fired Unit** | Trường “pre chain fired unit” của mỗi firing được tính bằng inverse fired unit cộng symmetric fired unit. |
| **DR-119 Pre Chain Order Key** | Trường “pre chain order key” của mỗi firing được tính bằng first sort order nhân 10 cộng kind sort order. |
| **DR-120 Round** | Trường “round” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 2, nếu tất cả các điều kiện sau đều đúng: kind là “chain” và không xảy ra việc tất cả các điều kiện sau đều đúng: cờ “first is in effect” được bật và cờ “second is in effect” được bật;<br>2. trong mọi trường hợp còn lại, 1. |
| **DR-121 Order Key** | Trường “order key” của mỗi firing được tính bằng round nhân 1000000000000000 cộng first snap position nhân 100000000 cộng kind sort order nhân 10000000 cộng 0 nếu second để trống, trong mọi trường hợp còn lại là second snap position. |
| **DR-122 Is Chosen** | Mỗi firing được coi là “chosen” nếu tất cả các điều kiện sau đều đúng: fired unit là 1, cờ “target is conclusion” được bật và order key là target first fired order key. |
| **DR-123 Chosen Unit** | Trường “chosen unit” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu cờ “chosen” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-124 Card Text** | Trường “card text” của mỗi firing được xác định theo thứ tự ưu tiên sau:<br>1. “Worked out by the ”, tiếp theo là kind wording, tiếp theo là “ from “”, tiếp theo là first words, tiếp theo là “””, tiếp theo là “ and “”, tiếp theo là second words, tiếp theo là “”” nếu kind là “chain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has domain ”, tiếp theo là first property domain class, tiếp theo là “”” nếu kind là “domain”, trong mọi trường hợp còn lại là “ and “”, tiếp theo là first property, tiếp theo là “ has range ”, tiếp theo là first property range class, tiếp theo là “””, tiếp theo là dấu chấm, nếu cờ “chosen” được bật;<br>2. trong mọi trường hợp còn lại, chuỗi rỗng. ⚠︎ máy móc <!-- rulespeak:reword --> |
| **DR-125 Name** | Trường “name” của mỗi firing có cùng giá trị với trường “firing ID” của chính firing đó. |
| **DR-126 Rule Enabled** | Trường “rule enabled” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “rule” là “enabled”. |
| **DR-127 First Holds** | Trường “first holds” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “first” thỏa điều kiện “holds”. |
| **DR-128 Second Holds** | Trường “second holds” của mỗi disjoint check có giá trị đúng khi bản ghi liên kết qua trường “second” thỏa điều kiện “holds”. |
| **DR-129 Subject Label** | Trường “subject label” của mỗi disjoint check được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-130 Object Label** | Trường “object label” của mỗi disjoint check được lấy từ bản ghi liên kết qua trường “first”. |
| **DR-131 Violation Unit** | Trường “violation unit” của mỗi disjoint check được xác định theo thứ tự ưu tiên sau:<br>1. 1, nếu tất cả các điều kiện sau đều đúng: cờ “rule enabled” được bật, cờ “first holds” được bật và cờ “second holds” được bật;<br>2. trong mọi trường hợp còn lại, 0. |
| **DR-132 Alarm Text** | Trường “alarm text” của mỗi disjoint check được xác định theo thứ tự ưu tiên sau:<br>1. “Contradiction found. ”, tiếp theo là subject label, tiếp theo là “ is recorded as both parent and spouse of ”, tiếp theo là object label, tiếp theo là “. Check those facts and the selected disjoint rule.”, nếu violation unit là 1;<br>2. trong mọi trường hợp còn lại, chuỗi rỗng. |
| **DR-133 Name** | Trường “name” của mỗi disjoint check có cùng giá trị với trường “disjoint check ID” của chính disjoint check đó. |
| **DR-134 Name** | Trường “name” của mỗi scenario có cùng giá trị với trường “label” của chính scenario đó. |
| **DR-135 Graph Has Facts to Add** | Trường “graph has facts to add” của mỗi scenario có giá trị đúng khi bản ghi liên kết qua trường “graph” có “facts to add”. |
| **DR-136 Is Available** | Mỗi scenario được coi là “available” nếu ít nhất một trong các điều kiện sau đúng: fact baseline khác “next” hoặc cờ “graph has facts to add” được bật. |
| **DR-137 Name** | Trường “name” của mỗi scenario fact change có cùng giá trị với trường “scenario fact change ID” của chính scenario fact change đó. |
| **DR-138 Name** | Trường “name” của mỗi scenario rule setting có cùng giá trị với trường “scenario rule setting ID” của chính scenario rule setting đó. |
| **DR-139 Name** | Trường “name” của mỗi node layout có cùng giá trị với trường “node layout ID” của chính node layout đó. |
| **DR-140 Name** | Trường “name” của mỗi layout slot có cùng giá trị với trường “layout slot ID” của chính layout slot đó. |
| **DR-141 Name** | Trường “name” của mỗi page text có cùng giá trị với trường “page text ID” của chính page text đó. |
| **DR-142 Name** | Trường “name” của mỗi engine có cùng giá trị với trường “label” của chính engine đó. |
| **DR-143 Name** | Trường “name” của mỗi rulebook tool có cùng giá trị với trường “label” của chính rulebook tool đó. |

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
| **Graphs.MaxSortOrder** | tổng hợp | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | công thức | `MaxSortOrder + 1` |
| **Rules.Name** | công thức | `Label` |
| **Rules.CountOfFirings** | tổng hợp | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | công thức | `Wording` |
| **FiringKinds.RuleEnabled** | tra cứu | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | tra cứu | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | tra cứu | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | tra cứu | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | tra cứu | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | tra cứu | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | tra cứu | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | công thức | `Label` |
| **Things.NodeKey** | công thức | `ThingId` |
| **Things.CountOfChildren** | tổng hợp | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | tổng hợp | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | tổng hợp | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | tổng hợp | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | tổng hợp | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | công thức | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | công thức | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | công thức | `DisplayName` |
| **PortraitAliases.Name** | công thức | `PortraitAliasId` |
| **Properties.Name** | công thức | `Label` |
| **PropertyAxioms.Name** | công thức | `PropertyAxiomId` |
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
| **Statements.HoldsBeforeChain** | công thức | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | tổng hợp | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | công thức | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | tổng hợp | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | công thức | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | công thức | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | công thức | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | công thức | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | công thức | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | công thức | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | công thức | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | tổng hợp | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | công thức | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | tổng hợp | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | tổng hợp | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | công thức | `Subject` |
| **Statements.ObjectNodeKey** | công thức | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | tra cứu | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | tra cứu | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | công thức | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | tra cứu | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | tổng hợp | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | tổng hợp | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | công thức | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | tra cứu | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | tra cứu | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | tra cứu | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | tra cứu | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | tra cứu | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | tra cứu | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | tra cứu | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | tra cứu | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | tra cứu | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | tra cứu | `Lookup(Statements.ObjectNodeKey via Target)` |
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
| **Firings.SecondIsInEffect** | tra cứu | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | tra cứu | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | tra cứu | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | công thức | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | công thức | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | công thức | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | công thức | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | công thức | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | công thức | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | công thức | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
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
| **Scenarios.Name** | công thức | `Label` |
| **Scenarios.GraphHasFactsToAdd** | tra cứu | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | công thức | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | công thức | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | công thức | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | công thức | `NodeLayoutId` |
| **LayoutSlots.Name** | công thức | `LayoutSlotId` |
| **PageTexts.Name** | công thức | `PageTextId` |
| **Engines.Name** | công thức | `Label` |
| **RulebookTools.Name** | công thức | `Label` |

---

> **RuleSpeak®** — ký pháp quy tắc nghiệp vụ khai báo do **Ronald G. Ross** sáng tạo, theo quy ước **SBVR** — sắp ra mắt. Trong lúc đó, trang này hiển thị tài liệu thông thường. [www.RonRoss.info](https://www.RonRoss.info)
