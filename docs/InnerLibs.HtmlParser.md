## `ClassList`

```csharp
public class InnerLibs.HtmlParser.ClassList
    : List<String>, IList<String>, ICollection<String>, IEnumerable<String>, IEnumerable, IList, ICollection, IReadOnlyList<String>, IReadOnlyCollection<String>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Count | Count the class of element | 
| `Boolean` | IsReadOnly |  | 
| `Boolean` | Item | Gets or sets a value indicating if this element contains a specifc class | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | Add(`String[]` ClassName) | Add a class to element | 
| `HtmlElement` | AddRange(`String[]` ClassName) | Add a class to element | 
| `void` | Clear() | Remove the class attribute from element | 
| `Boolean` | Contains(`String[]` ClassName) | Check if element coitains all the classes | 
| `void` | CopyTo(`String[]` array, `Int32` arrayIndex) |  | 
| `String` | FromExpression(`Object[]` ClassEx) | Proccess a set of objects and apply class names according to the boolean properties, keyvaluepairs or strings | 
| `IEnumerator<String>` | GetEnumerator() |  | 
| `Int32` | IndexOf(`String` ClassName) | Gets the class position index in element | 
| `void` | Insert(`Int32` index, `String` ClassName) | Insert a class into specific index | 
| `Boolean` | Remove(`String` ClassName) | Remove a class from element | 
| `void` | RemoveAt(`Int32` index) | Remove the class with specific index | 
| `String` | ToString() | Returns the class attribute of element | 


## `CssProperties`

```csharp
public class InnerLibs.HtmlParser.CssProperties
    : IDictionary<String, String>, ICollection<KeyValuePair<String, String>>, IEnumerable<KeyValuePair<String, String>>, IEnumerable

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | align_content |  | 
| `String` | align_items |  | 
| `String` | align_self |  | 
| `String` | animation |  | 
| `String` | animation_delay |  | 
| `String` | animation_direction |  | 
| `String` | animation_duration |  | 
| `String` | animation_fill_mode |  | 
| `String` | animation_iteration_count |  | 
| `String` | animation_name |  | 
| `String` | animation_play_state |  | 
| `String` | animation_timing_function |  | 
| `String` | backface_visibility |  | 
| `String` | background |  | 
| `String` | background_attachment |  | 
| `String` | background_clip |  | 
| `String` | background_color |  | 
| `String` | background_image |  | 
| `String` | background_origin |  | 
| `String` | background_position |  | 
| `String` | background_repeat |  | 
| `String` | background_size |  | 
| `String` | border |  | 
| `String` | border_bottom |  | 
| `String` | border_bottom_color |  | 
| `String` | border_bottom_left_radius |  | 
| `String` | border_bottom_right_radius |  | 
| `String` | border_bottom_style |  | 
| `String` | border_bottom_width |  | 
| `String` | border_collapse |  | 
| `String` | border_color |  | 
| `String` | border_image |  | 
| `String` | border_image_outset |  | 
| `String` | border_image_repeat |  | 
| `String` | border_image_slice |  | 
| `String` | border_image_source |  | 
| `String` | border_image_width |  | 
| `String` | border_left |  | 
| `String` | border_left_color |  | 
| `String` | border_left_style |  | 
| `String` | border_left_width |  | 
| `String` | border_radius |  | 
| `String` | border_right |  | 
| `String` | border_right_color |  | 
| `String` | border_right_style |  | 
| `String` | border_right_width |  | 
| `String` | border_spacing |  | 
| `String` | border_style |  | 
| `String` | border_top |  | 
| `String` | border_top_color |  | 
| `String` | border_top_left_radius |  | 
| `String` | border_top_right_radius |  | 
| `String` | border_top_style |  | 
| `String` | border_top_width |  | 
| `String` | border_width |  | 
| `String` | bottom |  | 
| `String` | box_shadow |  | 
| `String` | box_sizing |  | 
| `String` | caption_side |  | 
| `String` | clear |  | 
| `String` | clip |  | 
| `String` | color |  | 
| `String` | column_count |  | 
| `String` | column_fill |  | 
| `String` | column_gap |  | 
| `String` | column_rule |  | 
| `String` | column_rule_color |  | 
| `String` | column_rule_style |  | 
| `String` | column_rule_width |  | 
| `String` | column_span |  | 
| `String` | column_width |  | 
| `String` | columns |  | 
| `String` | content |  | 
| `Int32` | Count |  | 
| `String` | counter_increment |  | 
| `String` | counter_reset |  | 
| `String` | cursor |  | 
| `String` | direction |  | 
| `String` | display |  | 
| `String` | empty_cells |  | 
| `String` | flex |  | 
| `String` | flex_basis |  | 
| `String` | flex_direction |  | 
| `String` | flex_flow |  | 
| `String` | flex_grow |  | 
| `String` | flex_shrink |  | 
| `String` | flex_wrap |  | 
| `String` | float |  | 
| `String` | font |  | 
| `String` | font_family |  | 
| `String` | font_size |  | 
| `String` | font_size_adjust |  | 
| `String` | font_stretch |  | 
| `String` | font_style |  | 
| `String` | font_variant |  | 
| `String` | font_weight |  | 
| `String` | height |  | 
| `Boolean` | IsReadOnly |  | 
| `String` | Item | Gets or sets the style of element | 
| `String` | justify_content |  | 
| `ICollection<String>` | Keys |  | 
| `String` | left |  | 
| `String` | letter_spacing |  | 
| `String` | line_height |  | 
| `String` | list_style |  | 
| `String` | list_style_image |  | 
| `String` | list_style_position |  | 
| `String` | list_style_type |  | 
| `String` | margin |  | 
| `String` | margin_bottom |  | 
| `String` | margin_left |  | 
| `String` | margin_right |  | 
| `String` | margin_top |  | 
| `String` | max_height |  | 
| `String` | max_width |  | 
| `String` | min_height |  | 
| `String` | min_width |  | 
| `String` | opacity |  | 
| `String` | order |  | 
| `String` | outline |  | 
| `String` | outline_color |  | 
| `String` | outline_offset |  | 
| `String` | outline_style |  | 
| `String` | outline_width |  | 
| `String` | overflow |  | 
| `String` | overflow_x |  | 
| `String` | overflow_y |  | 
| `String` | padding |  | 
| `String` | padding_bottom |  | 
| `String` | padding_left |  | 
| `String` | padding_right |  | 
| `String` | padding_top |  | 
| `String` | page_break_after |  | 
| `String` | page_break_before |  | 
| `String` | page_break_inside |  | 
| `String` | perspective |  | 
| `String` | perspective_origin |  | 
| `String` | position |  | 
| `String` | quotes |  | 
| `String` | resize |  | 
| `String` | right |  | 
| `String` | tab_size |  | 
| `String` | table_layout |  | 
| `String` | text_align |  | 
| `String` | text_align_last |  | 
| `String` | text_decoration |  | 
| `String` | text_decoration_color |  | 
| `String` | text_decoration_line |  | 
| `String` | text_decoration_style |  | 
| `String` | text_indent |  | 
| `String` | text_justify |  | 
| `String` | text_overflow |  | 
| `String` | text_shadow |  | 
| `String` | text_transform |  | 
| `String` | top |  | 
| `String` | transform |  | 
| `String` | transform_origin |  | 
| `String` | transform_style |  | 
| `String` | transition |  | 
| `String` | transition_delay |  | 
| `String` | transition_duration |  | 
| `String` | transition_property |  | 
| `String` | transition_timing_function |  | 
| `ICollection<String>` | Values |  | 
| `String` | vertical_align |  | 
| `String` | visibility |  | 
| `String` | white_space |  | 
| `String` | width |  | 
| `String` | word_break |  | 
| `String` | word_spacing |  | 
| `String` | word_wrap |  | 
| `String` | z_index |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`String` key, `String` value) |  | 
| `void` | Add(`KeyValuePair<String, String>` item) |  | 
| `void` | ClearStyle() |  | 
| `Boolean` | Contains(`String[]` AttKey) |  | 
| `Boolean` | Contains(`KeyValuePair<String, String>` item) |  | 
| `Boolean` | ContainsKey(`String` key) |  | 
| `IEnumerator<KeyValuePair<String, String>>` | GetEnumerator() |  | 
| `Boolean` | Remove(`KeyValuePair<String, String>` item) |  | 
| `CssProperties` | Set(`String` StyleString) |  | 
| `void` | SetColor(`Color` Color) |  | 
| `String` | ToString() |  | 
| `Boolean` | TryGetValue(`String` key, `String&` value) |  | 


## `CssSelector`

```csharp
public abstract class InnerLibs.HtmlParser.CssSelector

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AllowTraverse |  | 
| `String` | Selector |  | 
| `IList<CssSelector>` | SubSelectors |  | 
| `String` | Token |  | 


## `Emoji`

```csharp
public class InnerLibs.HtmlParser.Emoji

```

Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | _100 |  | 
| `String` | _1234 |  | 
| `String` | _8Ball |  | 
| `String` | A |  | 
| `String` | Ab |  | 
| `String` | Abc |  | 
| `String` | Abcd |  | 
| `String` | Accept |  | 
| `String` | Aerial_Tramway |  | 
| `String` | Airplane |  | 
| `String` | Alarm_Clock |  | 
| `String` | Alien |  | 
| `String` | Ambulance |  | 
| `String` | Anchor |  | 
| `String` | Angel |  | 
| `String` | Anger |  | 
| `String` | Angry |  | 
| `String` | Anguished |  | 
| `String` | Ant |  | 
| `String` | Apple |  | 
| `String` | Aquarius |  | 
| `String` | Aries |  | 
| `String` | Arrow_Backward |  | 
| `String` | Arrow_Double_Down |  | 
| `String` | Arrow_Double_Up |  | 
| `String` | Arrow_Down |  | 
| `String` | Arrow_Down_Small |  | 
| `String` | Arrow_Forward |  | 
| `String` | Arrow_Heading_Down |  | 
| `String` | Arrow_Heading_Up |  | 
| `String` | Arrow_Left |  | 
| `String` | Arrow_Lower_Left |  | 
| `String` | Arrow_Lower_Right |  | 
| `String` | Arrow_Right |  | 
| `String` | Arrow_Right_Hook |  | 
| `String` | Arrow_Up |  | 
| `String` | Arrow_Up_Down |  | 
| `String` | Arrow_Up_Small |  | 
| `String` | Arrow_Upper_Left |  | 
| `String` | Arrow_Upper_Right |  | 
| `String` | Arrows_Clockwise |  | 
| `String` | Arrows_Counterclockwise |  | 
| `String` | Art |  | 
| `String` | Articulated_Lorry |  | 
| `String` | Astonished |  | 
| `String` | Athletic_Shoe |  | 
| `String` | Atm |  | 
| `String` | B |  | 
| `String` | Baby |  | 
| `String` | Baby_Bottle |  | 
| `String` | Baby_Chick |  | 
| `String` | Baby_Symbol |  | 
| `String` | Back |  | 
| `String` | Baggage_Claim |  | 
| `String` | Balloon |  | 
| `String` | Ballot_Box_With_Check |  | 
| `String` | Bamboo |  | 
| `String` | Banana |  | 
| `String` | Bangbang |  | 
| `String` | Bank |  | 
| `String` | Bar_Chart |  | 
| `String` | Barber |  | 
| `String` | Baseball |  | 
| `String` | Basketball |  | 
| `String` | Bath |  | 
| `String` | Bathtub |  | 
| `String` | Battery |  | 
| `String` | Bear |  | 
| `String` | Bee |  | 
| `String` | Beer |  | 
| `String` | Beers |  | 
| `String` | Beetle |  | 
| `String` | Beginner |  | 
| `String` | Bell |  | 
| `String` | Bento |  | 
| `String` | Bicyclist |  | 
| `String` | Bike |  | 
| `String` | Bikini |  | 
| `String` | Bird |  | 
| `String` | Birthday |  | 
| `String` | Black_Circle |  | 
| `String` | Black_Joker |  | 
| `String` | Black_Large_Square |  | 
| `String` | Black_Medium_Small_Square |  | 
| `String` | Black_Medium_Square |  | 
| `String` | Black_Nib |  | 
| `String` | Black_Small_Square |  | 
| `String` | Black_Square_Button |  | 
| `String` | Blossom |  | 
| `String` | Blowfish |  | 
| `String` | Blue_Book |  | 
| `String` | Blue_Car |  | 
| `String` | Blue_Heart |  | 
| `String` | Blush |  | 
| `String` | Boar |  | 
| `String` | Bomb |  | 
| `String` | Book |  | 
| `String` | Bookmark |  | 
| `String` | Bookmark_Tabs |  | 
| `String` | Books |  | 
| `String` | Boom |  | 
| `String` | Boot |  | 
| `String` | Bouquet |  | 
| `String` | Bow |  | 
| `String` | Bowling |  | 
| `String` | Boy |  | 
| `String` | Bread |  | 
| `String` | Bride_With_Veil |  | 
| `String` | Bridge_At_Night |  | 
| `String` | Briefcase |  | 
| `String` | Broken_Heart |  | 
| `String` | Bug |  | 
| `String` | Bulb |  | 
| `String` | Bullettrain_Front |  | 
| `String` | Bullettrain_Side |  | 
| `String` | Bus |  | 
| `String` | Busstop |  | 
| `String` | Bust_In_Silhouette |  | 
| `String` | Busts_In_Silhouette |  | 
| `String` | Cactus |  | 
| `String` | Cake |  | 
| `String` | Calendar |  | 
| `String` | Calling |  | 
| `String` | Camel |  | 
| `String` | Camera |  | 
| `String` | Cancer |  | 
| `String` | Candy |  | 
| `String` | Capital_Abcd |  | 
| `String` | Capricorn |  | 
| `String` | Card_Index |  | 
| `String` | Carousel_Horse |  | 
| `String` | Cat |  | 
| `String` | Cat2 |  | 
| `String` | Cd |  | 
| `String` | Chart |  | 
| `String` | Chart_With_Downwards_Trend |  | 
| `String` | Chart_With_Upwards_Trend |  | 
| `String` | Checkered_Flag |  | 
| `String` | Cherries |  | 
| `String` | Cherry_Blossom |  | 
| `String` | Chestnut |  | 
| `String` | Chicken |  | 
| `String` | Children_Crossing |  | 
| `String` | Chocolate_Bar |  | 
| `String` | Christmas_Tree |  | 
| `String` | Church |  | 
| `String` | Cinema |  | 
| `String` | Circus_Tent |  | 
| `String` | City_Sunrise |  | 
| `String` | City_Sunset |  | 
| `String` | Cl |  | 
| `String` | Clap |  | 
| `String` | Clapper |  | 
| `String` | Clipboard |  | 
| `String` | Clock1 |  | 
| `String` | Clock10 |  | 
| `String` | Clock1030 |  | 
| `String` | Clock11 |  | 
| `String` | Clock1130 |  | 
| `String` | Clock12 |  | 
| `String` | Clock1230 |  | 
| `String` | Clock130 |  | 
| `String` | Clock2 |  | 
| `String` | Clock230 |  | 
| `String` | Clock3 |  | 
| `String` | Clock330 |  | 
| `String` | Clock4 |  | 
| `String` | Clock430 |  | 
| `String` | Clock5 |  | 
| `String` | Clock530 |  | 
| `String` | Clock6 |  | 
| `String` | Clock630 |  | 
| `String` | Clock7 |  | 
| `String` | Clock730 |  | 
| `String` | Clock8 |  | 
| `String` | Clock830 |  | 
| `String` | Clock9 |  | 
| `String` | Clock930 |  | 
| `String` | Closed_Book |  | 
| `String` | Closed_Lock_With_Key |  | 
| `String` | Closed_Umbrella |  | 
| `String` | Cloud |  | 
| `String` | Clubs |  | 
| `String` | Cn |  | 
| `String` | Cocktail |  | 
| `String` | Coffee |  | 
| `String` | Cold_Sweat |  | 
| `String` | Computer |  | 
| `String` | Confetti_Ball |  | 
| `String` | Confounded |  | 
| `String` | Confused |  | 
| `String` | Congratulations |  | 
| `String` | Construction |  | 
| `String` | Construction_Worker |  | 
| `String` | Convenience_Store |  | 
| `String` | Cookie |  | 
| `String` | Cool |  | 
| `String` | Cop |  | 
| `String` | Copyright |  | 
| `String` | Corn |  | 
| `String` | Couple |  | 
| `String` | Couple_With_Heart |  | 
| `String` | Couplekiss |  | 
| `String` | Cow |  | 
| `String` | Cow2 |  | 
| `String` | Credit_Card |  | 
| `String` | Crescent_Moon |  | 
| `String` | Crocodile |  | 
| `String` | Crossed_Flags |  | 
| `String` | Crown |  | 
| `String` | Cry |  | 
| `String` | Crying_Cat_Face |  | 
| `String` | Crystal_Ball |  | 
| `String` | Cupid |  | 
| `String` | Curly_Loop |  | 
| `String` | Currency_Exchange |  | 
| `String` | Curry |  | 
| `String` | Custard |  | 
| `String` | Customs |  | 
| `String` | Cyclone |  | 
| `String` | Dancer |  | 
| `String` | Dancers |  | 
| `String` | Dango |  | 
| `String` | Dart |  | 
| `String` | Dash |  | 
| `String` | Date |  | 
| `String` | De |  | 
| `String` | Deciduous_Tree |  | 
| `String` | Department_Store |  | 
| `String` | Diamond_Shape_With_A_Dot_Inside |  | 
| `String` | Diamonds |  | 
| `String` | Disappointed |  | 
| `String` | Disappointed_Relieved |  | 
| `String` | Dizzy |  | 
| `String` | Dizzy_Face |  | 
| `String` | Do_Not_Litter |  | 
| `String` | Dog |  | 
| `String` | Dog2 |  | 
| `String` | Dollar |  | 
| `String` | Dolls |  | 
| `String` | Dolphin |  | 
| `String` | Door |  | 
| `String` | Doughnut |  | 
| `String` | Dragon |  | 
| `String` | Dragon_Face |  | 
| `String` | Dress |  | 
| `String` | Dromedary_Camel |  | 
| `String` | Droplet |  | 
| `String` | Dvd |  | 
| `String` | E_Mail |  | 
| `String` | Ear |  | 
| `String` | Ear_Of_Rice |  | 
| `String` | Earth_Africa |  | 
| `String` | Earth_Americas |  | 
| `String` | Earth_Asia |  | 
| `String` | Egg |  | 
| `String` | Eggplant |  | 
| `String` | Eight |  | 
| `String` | Eight_Pointed_Black_Star |  | 
| `String` | Eight_Spoked_Asterisk |  | 
| `String` | Electric_Plug |  | 
| `String` | Elephant |  | 
| `String` | End |  | 
| `String` | Envelope |  | 
| `String` | Envelope_With_Arrow |  | 
| `String` | Es |  | 
| `String` | Euro |  | 
| `String` | European_Castle |  | 
| `String` | European_Post_Office |  | 
| `String` | Evergreen_Tree |  | 
| `String` | Exclamation |  | 
| `String` | Expressionless |  | 
| `String` | Eyeglasses |  | 
| `String` | Eyes |  | 
| `String` | Factory |  | 
| `String` | Fallen_Leaf |  | 
| `String` | Family |  | 
| `String` | Fast_Forward |  | 
| `String` | Fax |  | 
| `String` | Fearful |  | 
| `String` | Feet |  | 
| `String` | Ferris_Wheel |  | 
| `String` | File_Folder |  | 
| `String` | Fire |  | 
| `String` | Fire_Engine |  | 
| `String` | Fireworks |  | 
| `String` | First_Quarter_Moon |  | 
| `String` | First_Quarter_Moon_With_Face |  | 
| `String` | Fish |  | 
| `String` | Fish_Cake |  | 
| `String` | Fishing_Pole_And_Fish |  | 
| `String` | Fist |  | 
| `String` | Five |  | 
| `String` | Flags |  | 
| `String` | Flashlight |  | 
| `String` | Floppy_Disk |  | 
| `String` | Flower_Playing_Cards |  | 
| `String` | Flushed |  | 
| `String` | Foggy |  | 
| `String` | Football |  | 
| `String` | Footprints |  | 
| `String` | Fork_And_Knife |  | 
| `String` | Fountain |  | 
| `String` | Four |  | 
| `String` | Four_Leaf_Clover |  | 
| `String` | Fr |  | 
| `String` | Free |  | 
| `String` | Fried_Shrimp |  | 
| `String` | Fries |  | 
| `String` | Frog |  | 
| `String` | Frowning |  | 
| `String` | Fuelpump |  | 
| `String` | Full_Moon |  | 
| `String` | Full_Moon_With_Face |  | 
| `String` | Game_Die |  | 
| `String` | Gem |  | 
| `String` | Gemini |  | 
| `String` | Ghost |  | 
| `String` | Gift |  | 
| `String` | Gift_Heart |  | 
| `String` | Girl |  | 
| `String` | Globe_With_Meridians |  | 
| `String` | Goat |  | 
| `String` | Golf |  | 
| `String` | Grapes |  | 
| `String` | Green_Apple |  | 
| `String` | Green_Book |  | 
| `String` | Green_Heart |  | 
| `String` | Grey_Exclamation |  | 
| `String` | Grey_Question |  | 
| `String` | Grimacing |  | 
| `String` | Grin |  | 
| `String` | Grinning |  | 
| `String` | Guardsman |  | 
| `String` | Guitar |  | 
| `String` | Gun |  | 
| `String` | Haircut |  | 
| `String` | Hamburger |  | 
| `String` | Hammer |  | 
| `String` | Hamster |  | 
| `String` | Handbag |  | 
| `String` | Hash |  | 
| `String` | Hatched_Chick |  | 
| `String` | Hatching_Chick |  | 
| `String` | Headphones |  | 
| `String` | Hear_No_Evil |  | 
| `String` | Heart |  | 
| `String` | Heart_Decoration |  | 
| `String` | Heart_Eyes |  | 
| `String` | Heart_Eyes_Cat |  | 
| `String` | Heartbeat |  | 
| `String` | Heartpulse |  | 
| `String` | Hearts |  | 
| `String` | Heavy_Check_Mark |  | 
| `String` | Heavy_Division_Sign |  | 
| `String` | Heavy_Dollar_Sign |  | 
| `String` | Heavy_Minus_Sign |  | 
| `String` | Heavy_Multiplication_X |  | 
| `String` | Heavy_Plus_Sign |  | 
| `String` | Helicopter |  | 
| `String` | Herb |  | 
| `String` | Hibiscus |  | 
| `String` | High_Brightness |  | 
| `String` | High_Heel |  | 
| `String` | Honey_Pot |  | 
| `String` | Horse |  | 
| `String` | Horse_Racing |  | 
| `String` | Hospital |  | 
| `String` | Hotel |  | 
| `String` | Hotsprings |  | 
| `String` | Hourglass |  | 
| `String` | Hourglass_Flowing_Sand |  | 
| `String` | House |  | 
| `String` | House_With_Garden |  | 
| `String` | Hushed |  | 
| `String` | Ice_Cream |  | 
| `String` | Icecream |  | 
| `String` | Id |  | 
| `String` | Ideograph_Advantage |  | 
| `String` | Imp |  | 
| `String` | Inbox_Tray |  | 
| `String` | Incoming_Envelope |  | 
| `String` | Information_Desk_Person |  | 
| `String` | Information_Source |  | 
| `String` | Innocent |  | 
| `String` | Interrobang |  | 
| `String` | Iphone |  | 
| `String` | It |  | 
| `String` | Izakaya_Lantern |  | 
| `String` | Jack_O_Lantern |  | 
| `String` | Japan |  | 
| `String` | Japanese_Castle |  | 
| `String` | Japanese_Goblin |  | 
| `String` | Japanese_Ogre |  | 
| `String` | Jeans |  | 
| `String` | Joy |  | 
| `String` | Joy_Cat |  | 
| `String` | Jp |  | 
| `String` | Key |  | 
| `String` | Keycap_Ten |  | 
| `String` | Kimono |  | 
| `String` | Kiss |  | 
| `String` | Kissing |  | 
| `String` | Kissing_Cat |  | 
| `String` | Kissing_Closed_Eyes |  | 
| `String` | Kissing_Heart |  | 
| `String` | Kissing_Smiling_Eyes |  | 
| `String` | Knife |  | 
| `String` | Koala |  | 
| `String` | Koko |  | 
| `String` | Kr |  | 
| `String` | Large_Blue_Circle |  | 
| `String` | Large_Blue_Diamond |  | 
| `String` | Large_Orange_Diamond |  | 
| `String` | Last_Quarter_Moon |  | 
| `String` | Last_Quarter_Moon_With_Face |  | 
| `String` | Laughing |  | 
| `String` | Leaves |  | 
| `String` | Ledger |  | 
| `String` | Left_Luggage |  | 
| `String` | Left_Right_Arrow |  | 
| `String` | Leftwards_Arrow_With_Hook |  | 
| `String` | Lemon |  | 
| `String` | Leo |  | 
| `String` | Leopard |  | 
| `String` | Libra |  | 
| `String` | Light_Rail |  | 
| `String` | Link |  | 
| `String` | Lips |  | 
| `String` | Lipstick |  | 
| `String` | Lock |  | 
| `String` | Lock_With_Ink_Pen |  | 
| `String` | Lollipop |  | 
| `String` | Loop |  | 
| `String` | Loud_Sound |  | 
| `String` | Loudspeaker |  | 
| `String` | Love_Hotel |  | 
| `String` | Love_Letter |  | 
| `String` | Low_Brightness |  | 
| `String` | M |  | 
| `String` | Mag |  | 
| `String` | Mag_Right |  | 
| `String` | Mahjong |  | 
| `String` | Mailbox |  | 
| `String` | Mailbox_Closed |  | 
| `String` | Mailbox_With_Mail |  | 
| `String` | Mailbox_With_No_Mail |  | 
| `String` | Man |  | 
| `String` | Man_With_Gua_Pi_Mao |  | 
| `String` | Man_With_Turban |  | 
| `String` | Mans_Shoe |  | 
| `String` | Maple_Leaf |  | 
| `String` | Mask |  | 
| `String` | Massage |  | 
| `String` | Meat_On_Bone |  | 
| `String` | Mega |  | 
| `String` | Melon |  | 
| `String` | Mens |  | 
| `String` | Metro |  | 
| `String` | Microphone |  | 
| `String` | Microscope |  | 
| `String` | Milky_Way |  | 
| `String` | Minibus |  | 
| `String` | Minidisc |  | 
| `String` | Mobile_Phone_Off |  | 
| `String` | Money_With_Wings |  | 
| `String` | Moneybag |  | 
| `String` | Monkey |  | 
| `String` | Monkey_Face |  | 
| `String` | Monorail |  | 
| `String` | Mortar_Board |  | 
| `String` | Mount_Fuji |  | 
| `String` | Mountain_Bicyclist |  | 
| `String` | Mountain_Cableway |  | 
| `String` | Mountain_Railway |  | 
| `String` | Mouse |  | 
| `String` | Mouse2 |  | 
| `String` | Movie_Camera |  | 
| `String` | Moyai |  | 
| `String` | Muscle |  | 
| `String` | Mushroom |  | 
| `String` | Musical_Keyboard |  | 
| `String` | Musical_Note |  | 
| `String` | Musical_Score |  | 
| `String` | Mute |  | 
| `String` | Nail_Care |  | 
| `String` | Name_Badge |  | 
| `String` | Necktie |  | 
| `String` | Negative_Squared_Cross_Mark |  | 
| `String` | Neutral_Face |  | 
| `String` | New |  | 
| `String` | New_Moon |  | 
| `String` | New_Moon_With_Face |  | 
| `String` | Newspaper |  | 
| `String` | Ng |  | 
| `String` | Night_With_Stars |  | 
| `String` | Nine |  | 
| `String` | No_Bell |  | 
| `String` | No_Bicycles |  | 
| `String` | No_Entry |  | 
| `String` | No_Entry_Sign |  | 
| `String` | No_Good |  | 
| `String` | No_Mobile_Phones |  | 
| `String` | No_Mouth |  | 
| `String` | No_Pedestrians |  | 
| `String` | No_Smoking |  | 
| `String` | Non_Potable_Water |  | 
| `String` | Nose |  | 
| `String` | Notebook |  | 
| `String` | Notebook_With_Decorative_Cover |  | 
| `String` | Notes |  | 
| `String` | Nut_And_Bolt |  | 
| `String` | O |  | 
| `String` | O2 |  | 
| `String` | Ocean |  | 
| `String` | Octopus |  | 
| `String` | Oden |  | 
| `String` | Office |  | 
| `String` | Ok |  | 
| `String` | Ok_Hand |  | 
| `String` | Ok_Woman |  | 
| `String` | Older_Man |  | 
| `String` | Older_Woman |  | 
| `String` | On |  | 
| `String` | Oncoming_Automobile |  | 
| `String` | Oncoming_Bus |  | 
| `String` | Oncoming_Police_Car |  | 
| `String` | Oncoming_Taxi |  | 
| `String` | One |  | 
| `String` | Open_File_Folder |  | 
| `String` | Open_Hands |  | 
| `String` | Open_Mouth |  | 
| `String` | Ophiuchus |  | 
| `String` | Orange_Book |  | 
| `String` | Outbox_Tray |  | 
| `String` | Ox |  | 
| `String` | Package |  | 
| `String` | Page_Facing_Up |  | 
| `String` | Page_With_Curl |  | 
| `String` | Pager |  | 
| `String` | Palm_Tree |  | 
| `String` | Panda_Face |  | 
| `String` | Paperclip |  | 
| `String` | Parking |  | 
| `String` | Part_Alternation_Mark |  | 
| `String` | Partly_Sunny |  | 
| `String` | Passport_Control |  | 
| `String` | Peach |  | 
| `String` | Pear |  | 
| `String` | Pencil |  | 
| `String` | Pencil2 |  | 
| `String` | Penguin |  | 
| `String` | Pensive |  | 
| `String` | Performing_Arts |  | 
| `String` | Persevere |  | 
| `String` | Person_Frowning |  | 
| `String` | Person_With_Blond_Hair |  | 
| `String` | Person_With_Pouting_Face |  | 
| `String` | Pig |  | 
| `String` | Pig_Nose |  | 
| `String` | Pig2 |  | 
| `String` | Pill |  | 
| `String` | Pineapple |  | 
| `String` | Pisces |  | 
| `String` | Pizza |  | 
| `String` | Point_Down |  | 
| `String` | Point_Left |  | 
| `String` | Point_Right |  | 
| `String` | Point_Up |  | 
| `String` | Point_Up_2 |  | 
| `String` | Police_Car |  | 
| `String` | Poodle |  | 
| `String` | Poop |  | 
| `String` | Post_Office |  | 
| `String` | Postal_Horn |  | 
| `String` | Postbox |  | 
| `String` | Potable_Water |  | 
| `String` | Pouch |  | 
| `String` | Poultry_Leg |  | 
| `String` | Pound |  | 
| `String` | Pouting_Cat |  | 
| `String` | Pray |  | 
| `String` | Princess |  | 
| `String` | Punch |  | 
| `String` | Purple_Heart |  | 
| `String` | Purse |  | 
| `String` | Pushpin |  | 
| `String` | Put_Litter_In_Its_Place |  | 
| `String` | Question |  | 
| `String` | Rabbit |  | 
| `String` | Rabbit2 |  | 
| `String` | Racehorse |  | 
| `String` | Radio |  | 
| `String` | Radio_Button |  | 
| `String` | Rage |  | 
| `String` | Railway_Car |  | 
| `String` | Rainbow |  | 
| `String` | Raised_Hand |  | 
| `String` | Raised_Hands |  | 
| `String` | Raising_Hand |  | 
| `String` | Ram |  | 
| `String` | Ramen |  | 
| `String` | Rat |  | 
| `String` | Recycle |  | 
| `String` | Red_Car |  | 
| `String` | Red_Circle |  | 
| `String` | Registered |  | 
| `String` | Relaxed |  | 
| `String` | Relieved |  | 
| `String` | Repeat |  | 
| `String` | Repeat_One |  | 
| `String` | Restroom |  | 
| `String` | Revolving_Hearts |  | 
| `String` | Rewind |  | 
| `String` | Ribbon |  | 
| `String` | Rice |  | 
| `String` | Rice_Ball |  | 
| `String` | Rice_Cracker |  | 
| `String` | Rice_Scene |  | 
| `String` | Ring |  | 
| `String` | Robot_Face |  | 
| `String` | Rocket |  | 
| `String` | Roller_Coaster |  | 
| `String` | Rooster |  | 
| `String` | Rose |  | 
| `String` | Rotating_Light |  | 
| `String` | Round_Pushpin |  | 
| `String` | Rowboat |  | 
| `String` | Ru |  | 
| `String` | Rugby_Football |  | 
| `String` | Runner |  | 
| `String` | Running_Shirt_With_Sash |  | 
| `String` | Sa |  | 
| `String` | Sagittarius |  | 
| `String` | Sailboat |  | 
| `String` | Sake |  | 
| `String` | Sandal |  | 
| `String` | Santa |  | 
| `String` | Satellite |  | 
| `String` | Saxophone |  | 
| `String` | School |  | 
| `String` | School_Satchel |  | 
| `String` | Scissors |  | 
| `String` | Scorpius |  | 
| `String` | Scream |  | 
| `String` | Scream_Cat |  | 
| `String` | Scroll |  | 
| `String` | Seat |  | 
| `String` | Secret |  | 
| `String` | See_No_Evil |  | 
| `String` | Seedling |  | 
| `String` | Seven |  | 
| `String` | Shaved_Ice |  | 
| `String` | Sheep |  | 
| `String` | Shell |  | 
| `String` | Ship |  | 
| `String` | Shirt |  | 
| `String` | Shower |  | 
| `String` | Signal_Strength |  | 
| `String` | Six |  | 
| `String` | Six_Pointed_Star |  | 
| `String` | Ski |  | 
| `String` | Skull |  | 
| `String` | Sleeping |  | 
| `String` | Sleepy |  | 
| `String` | Slot_Machine |  | 
| `String` | Small_Blue_Diamond |  | 
| `String` | Small_Orange_Diamond |  | 
| `String` | Small_Red_Triangle |  | 
| `String` | Small_Red_Triangle_Down |  | 
| `String` | Smile |  | 
| `String` | Smile_Cat |  | 
| `String` | Smiley |  | 
| `String` | Smiley_Cat |  | 
| `String` | Smiling_Imp |  | 
| `String` | Smirk |  | 
| `String` | Smirk_Cat |  | 
| `String` | Smoking |  | 
| `String` | Snail |  | 
| `String` | Snake |  | 
| `String` | Snowboarder |  | 
| `String` | Snowflake |  | 
| `String` | Snowman |  | 
| `String` | Sob |  | 
| `String` | Soccer |  | 
| `String` | Soon |  | 
| `String` | Sos |  | 
| `String` | Sound |  | 
| `String` | Space_Invader |  | 
| `String` | Spades |  | 
| `String` | Spaghetti |  | 
| `String` | Sparkle |  | 
| `String` | Sparkler |  | 
| `String` | Sparkles |  | 
| `String` | Sparkling_Heart |  | 
| `String` | Speak_No_Evil |  | 
| `String` | Speaker |  | 
| `String` | Speech_Balloon |  | 
| `String` | Speedboat |  | 
| `String` | Star |  | 
| `String` | Star2 |  | 
| `String` | Stars |  | 
| `String` | Station |  | 
| `String` | Statue_Of_Liberty |  | 
| `String` | Steam_Locomotive |  | 
| `String` | Stew |  | 
| `String` | Straight_Ruler |  | 
| `String` | Strawberry |  | 
| `String` | Stuck_Out_Tongue |  | 
| `String` | Stuck_Out_Tongue_Closed_Eyes |  | 
| `String` | Stuck_Out_Tongue_Winking_Eye |  | 
| `String` | Sun_With_Face |  | 
| `String` | Sunflower |  | 
| `String` | Sunglasses |  | 
| `String` | Sunny |  | 
| `String` | Sunrise |  | 
| `String` | Sunrise_Over_Mountains |  | 
| `String` | Surfer |  | 
| `String` | Sushi |  | 
| `String` | Suspension_Railway |  | 
| `String` | Sweat |  | 
| `String` | Sweat_Drops |  | 
| `String` | Sweat_Smile |  | 
| `String` | Sweet_Potato |  | 
| `String` | Swimmer |  | 
| `String` | Symbols |  | 
| `String` | Syringe |  | 
| `String` | Tada |  | 
| `String` | Tanabata_Tree |  | 
| `String` | Tangerine |  | 
| `String` | Taurus |  | 
| `String` | Taxi |  | 
| `String` | Tea |  | 
| `String` | Telephone |  | 
| `String` | Telephone_Receiver |  | 
| `String` | Telescope |  | 
| `String` | Tennis |  | 
| `String` | Tent |  | 
| `String` | Thought_Balloon |  | 
| `String` | Three |  | 
| `String` | Thumbsdown |  | 
| `String` | Thumbsup |  | 
| `String` | Ticket |  | 
| `String` | Tiger |  | 
| `String` | Tiger2 |  | 
| `String` | Tired_Face |  | 
| `String` | Tm |  | 
| `String` | Toilet |  | 
| `String` | Tokyo_Tower |  | 
| `String` | Tomato |  | 
| `String` | Tongue |  | 
| `String` | Top |  | 
| `String` | Tophat |  | 
| `String` | Tractor |  | 
| `String` | Traffic_Light |  | 
| `String` | Train |  | 
| `String` | Train2 |  | 
| `String` | Tram |  | 
| `String` | Triangular_Flag_On_Post |  | 
| `String` | Triangular_Ruler |  | 
| `String` | Trident |  | 
| `String` | Triumph |  | 
| `String` | Trolleybus |  | 
| `String` | Trophy |  | 
| `String` | Tropical_Drink |  | 
| `String` | Tropical_Fish |  | 
| `String` | Truck |  | 
| `String` | Trumpet |  | 
| `String` | Tulip |  | 
| `String` | Turtle |  | 
| `String` | Tv |  | 
| `String` | Twisted_Rightwards_Arrows |  | 
| `String` | Two |  | 
| `String` | Two_Hearts |  | 
| `String` | Two_Men_Holding_Hands |  | 
| `String` | Two_Women_Holding_Hands |  | 
| `String` | U5272 |  | 
| `String` | U5408 |  | 
| `String` | U55B6 |  | 
| `String` | U6307 |  | 
| `String` | U6708 |  | 
| `String` | U6709 |  | 
| `String` | U6E80 |  | 
| `String` | U7121 |  | 
| `String` | U7533 |  | 
| `String` | U7981 |  | 
| `String` | U7A7A |  | 
| `String` | Uk |  | 
| `String` | Umbrella |  | 
| `String` | Unamused |  | 
| `String` | Underage |  | 
| `String` | Unlock |  | 
| `String` | Up |  | 
| `String` | Us |  | 
| `String` | V |  | 
| `String` | Vertical_Traffic_Light |  | 
| `String` | Vhs |  | 
| `String` | Vibration_Mode |  | 
| `String` | Video_Camera |  | 
| `String` | Video_Game |  | 
| `String` | Violin |  | 
| `String` | Virgo |  | 
| `String` | Volcano |  | 
| `String` | Vs |  | 
| `String` | Walking |  | 
| `String` | Waning_Crescent_Moon |  | 
| `String` | Waning_Gibbous_Moon |  | 
| `String` | Warning |  | 
| `String` | Watch |  | 
| `String` | Water_Buffalo |  | 
| `String` | Watermelon |  | 
| `String` | Wave |  | 
| `String` | Wavy_Dash |  | 
| `String` | Waxing_Crescent_Moon |  | 
| `String` | Waxing_Gibbous_Moon |  | 
| `String` | Wc |  | 
| `String` | Weary |  | 
| `String` | Wedding |  | 
| `String` | Whale |  | 
| `String` | Whale2 |  | 
| `String` | Wheelchair |  | 
| `String` | White_Check_Mark |  | 
| `String` | White_Circle |  | 
| `String` | White_Flower |  | 
| `String` | White_Large_Square |  | 
| `String` | White_Medium_Small_Square |  | 
| `String` | White_Medium_Square |  | 
| `String` | White_Small_Square |  | 
| `String` | White_Square_Button |  | 
| `String` | Wind_Chime |  | 
| `String` | Wine_Glass |  | 
| `String` | Wink |  | 
| `String` | Wolf |  | 
| `String` | Woman |  | 
| `String` | Womans_Clothes |  | 
| `String` | Womans_Hat |  | 
| `String` | Womens |  | 
| `String` | Worried |  | 
| `String` | Wrench |  | 
| `String` | X |  | 
| `String` | Yellow_Heart |  | 
| `String` | Yen |  | 
| `String` | Yum |  | 
| `String` | Zap |  | 
| `String` | Zero |  | 
| `String` | Zzz |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetByName(`String` Name) |  | 
| `Dictionary<String, String>` | GetList() |  | 
| `Object` | ReplaceFaces(`HtmlElement&` Tag, `Func<String, String>` Method = null) |  | 
| `String` | ReplaceFaces(`String` Text, `Func<String, String>` Method = null) |  | 


## `HtmlAnchorElement`

```csharp
public class InnerLibs.HtmlParser.HtmlAnchorElement
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Href |  | 
| `String` | Name |  | 
| `String` | Target |  | 


## `HtmlAttribute`

The HtmlAttribute object represents a named value associated with an HtmlElement.
```csharp
public class InnerLibs.HtmlParser.HtmlAttribute

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | HasValue |  | 
| `String` | HTML |  | 
| `Boolean` | IsData |  | 
| `Boolean` | IsMinimized |  | 
| `String` | Name | The name of the attribute. e.g. WIDTH | 
| `String` | Value | The value of the attribute. e.g. 100% | 
| `String` | XHTML |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OutputType` | GetValue() |  | 
| `String` | ToString() | This will return an HTML-formatted version of this attribute. NB. This is  not SGML or XHTML safe, as it caters for null-value attributes such as "NOWRAP". | 


## `HtmlAttributeCollection`

This is a collection of attributes. Typically, this is associated with a particular  element. This collection is searchable by both the index and the name of the attribute.
```csharp
public class InnerLibs.HtmlParser.HtmlAttributeCollection
    : List<HtmlAttribute>, IList<HtmlAttribute>, ICollection<HtmlAttribute>, IEnumerable<HtmlAttribute>, IEnumerable, IList, ICollection, IReadOnlyList<HtmlAttribute>, IReadOnlyCollection<HtmlAttribute>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlAttribute` | Item | This overload allows you to have direct access to an attribute by providing  its name. If the attribute does not exist, null is returned. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlAttribute` | Add(`String` Name, `String` Value = null) | Add an attribute to element | 
| `HtmlAttribute` | Add(`HtmlAttribute` Attribute) | Add an attribute to element | 
| `HtmlAttributeCollection` | AddRange(`HtmlAttribute[]` Attributes) | Add multiple attributes to element | 
| `Boolean` | Contains(`String` Name) |  | 
| `HtmlAttribute` | FindByName(`String` Name) | This will search the collection for the named attribute. If it is not found, this  will return null. | 
| `void` | Remove(`String[]` Name) |  | 
| `Dictionary<String, String>` | ToDictionary() | Return a dictionary of this HtmlAttributeCollection | 


## `HtmlBreakLine`

```csharp
public class InnerLibs.HtmlParser.HtmlBreakLine
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Name |  | 


## `HtmlDocument`

This is the basic HTML document object used to represent a sequence of HTML.
```csharp
public class InnerLibs.HtmlParser.HtmlDocument

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | Body | Return the body element if exist | 
| `String` | DocTypeXHTML |  | 
| `String` | DocumentTitle |  | 
| `Encoding` | Encoding | The Encoding used to export this document as file | 
| `HtmlElement` | Head | Return the Head element if exist | 
| `String` | HTML | This will return the HTML used to represent this document. | 
| `String` | InnerHTML | Return a html string of child nodes | 
| `String` | InnerText | Returns the text of all child nodes | 
| `HtmlNodeCollection` | Nodes | This is the collection of nodes used to represent this document. | 
| `HtmlNodeCollection` | QuerySelectorAll | Travesse DOM with a CSS selector an retireve nodes | 
| `HtmlNodeCollection` | QuerySelectorAll | Travesse DOM with a CSS selector an retireve nodes | 
| `HtmlElement` | Title | Return the Title element if exist | 
| `String` | XHTML | This will return the XHTML document used to represent this document. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | CopyTo(`Stream&` s) | Copy this document to stream | 
| `HtmlNodeCollection` | FindElements(`Func<NodeType, Boolean>` predicate = null, `Boolean` SearchChildren = True) | This will search though this collection of nodes for all elements with matchs the predicate. | 
| `void` | FixText() | Fix the captalization, white spaces and punctuation of text elements | 
| `Byte[]` | GetBytes() | Return the byte array for this document | 
| `HtmlNodeCollection` | GetTextElements(`Boolean` SearchChildren = True) | Returns all Text elements excluding style and script elements | 
| `void` | LoadInto(`WebBrowser&` WebBrowser) | Load the document in `System.Windows.Forms.WebBrowser` control | 
| `void` | LoadInto(`TreeView&` TreeView) | Load the document in `System.Windows.Forms.WebBrowser` control | 
| `void` | MinifyCSS() | Merge all STYLE tags and minify it | 
| `HtmlElement` | QuerySelector(`String` CssSelector) | Travesse DOM with a CSS selector an retireve the first node | 
| `FileInfo` | SaveAs(`FileInfo` File) | Save the document as file | 
| `FileInfo` | SaveAs(`String` FileName) | Save the document as file | 
| `String` | ToJSON() | Return a Json representation of this element | 
| `String` | ToString() | Return the HTML of this document | 
| `XmlDocument` | ToXmlDocument() | Return the `System.Xml.XmlDocument` equivalent to this document | 


Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Html5Structure |  | 


## `HtmlElement`

The HtmlElement object represents any HTML element. An element has a name and zero or more attributes.
```csharp
public class InnerLibs.HtmlParser.HtmlElement
    : HtmlNode

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Attribute | Return the value of specific attibute | 
| `HtmlAttributeCollection` | Attributes | This is the collection of attributes associated with this element. | 
| `IEnumerable<String>` | AttributesNames | Return the name of al attributes | 
| `IEnumerable<HtmlElement>` | ChildElements | Return the child elements of this element (excluding HtmlText) | 
| `ClassList` | Class | The class list of this element | 
| `IEnumerable<HtmlText>` | ContentText | Return thedirect child text of this element (excluding HtmlElement) | 
| `String` | Data | Return the value of specific data-attribute | 
| `Boolean` | Disabled | Gets os sets a value indicating thats element is disabled | 
| `String` | ElementRepresentation | This will return the HTML representation of this element. | 
| `String` | HTML | This will return the HTML for this element and all subnodes. | 
| `String` | ID | The ID of element | 
| `String` | InnerHTML | Return a html string of child nodes | 
| `String` | InnerText |  | 
| `Boolean` | IsExplicitlyTerminated | This flag indicates that the element is explicitly closed using the /name method. | 
| `Boolean` | IsTerminated |  | 
| `Boolean` | IsVisible |  | 
| `String` | Name | This is the tag name of the element. e.g. BR, BODY, TABLE etc. | 
| `HtmlNodeCollection` | Nodes | This is the collection of all child nodes of this one. If this node is actually a text  node, this will return nothing. | 
| `HtmlNodeCollection` | QuerySelectorAll | Travesse element with a CSS selector an retireve nodes | 
| `CssProperties` | Style | The CSS style of element | 
| `String` | Title | Gets or sets the Title attribute of this element | 
| `String` | XHTML | This will return the XHTML for this element and all subnodes. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | AddAttribute(`String` Name, `String` Value = null) | Add a attribute to this element | 
| `void` | AddElement(`String[]` Names) | Add one or more empty elements by their tagnames | 
| `void` | AddNode(`HtmlNode[]` Node) | Add a node (or nodes) to collection | 
| `void` | AddNode(`HtmlGenericControl` Node) | Add a node (or nodes) to collection | 
| `void` | AddNode(`String` HTML, `Int32` Index = 0) | Add a node (or nodes) to collection | 
| `Boolean` | Censor(`Char` CensorChar, `String[]` BadWords) | Replace Badwords in all text elements. | 
| `HtmlElement` | Children(`String` CssSelector = ) | Return the first child element thats match de CssSelector | 
| `ElementType` | Children(`String` CssSelector = ) | Return the first child element thats match de CssSelector | 
| `HtmlElement` | Clone() | Clone this element into a new HtmlElement | 
| `Type` | CreateWebFormControl() | Create a `System.Web.UI.HtmlControls.HtmlControl` using this `InnerLibs.HtmlParser.HtmlElement` as source | 
| `Boolean` | Destroy() | Remove this element from parent element. If parent element is null, nothing happens | 
| `HtmlNodeCollection` | FindElements(`Func<NodeType, Boolean>` predicate, `Boolean` SearchChildren = True) | This will search though this collection of nodes for all elements with matchs the predicate. | 
| `void` | FixText() | Fix the punctuation, white spaces and captalization of the child text elements | 
| `HtmlNodeCollection` | GetTextElements(`Boolean` SearchChildren = True) | Returns all Text elements excluding style and script elements | 
| `Boolean` | HasAttribute(`String` Name) | Verify if this element has an specific attribute | 
| `Boolean` | HasClass(`String` ClassName = ) | Verify if this element has an specific class | 
| `void` | Mutate(`HtmlNodeCollection` Elements) | Transform the current element into a new set of elements | 
| `void` | Mutate(`HtmlElement` Element) | Transform the current element into a new set of elements | 
| `void` | Mutate(`String` Html) | Transform the current element into a new set of elements | 
| `void` | ParseEmoji(`Func<String, String>` Method = null, `Boolean` SearchChildren = True) | Find :emoji: and replace then using specific method | 
| `void` | ParseHashTags(`Func<String, String>` Method, `Boolean` SearchChildren = True) |  | 
| `void` | ParseMentionByChar(`String` MatchChar, `Func<String, String>` Method, `Boolean` SearchChildren = True) | Find mentions and replace then using specific method using a custom char on match | 
| `void` | ParseOEmbed(`Boolean` SearchChildren = True) | Find URLs out of Anchor elements and replace then to their respective oEmbed | 
| `void` | ParseURL(`Boolean` SearchChildren = True, `String` Target = _self) | Find URLs out of Anchor elements and replace then to anchors | 
| `void` | ParseUsername(`Func<String, String>` Method, `Boolean` SearchChildren = True) | Find @mentions and replace then using specific method | 
| `HtmlElement` | RemoveAttribute(`String` Name) | Remove an attribute from element | 
| `void` | SideClone(`Int32` Index = -1) | Clone this element into a new HtmlElement and inserts into same parent | 
| `HtmlElement` | ToggleClass(`String` ClassName, `Boolean` Status) | Gets os sets a boolean value for toggle an specific class | 
| `HtmlElement` | ToggleClass(`String` ClassName) | Gets os sets a boolean value for toggle an specific class | 
| `String` | ToString() | This will return the HTML representation of this element. | 
| `XmlElement` | ToXmlElement() | Return the `System.Xml.XmlElement` equivalent to this node | 


## `HtmlHorizontalRule`

```csharp
public class InnerLibs.HtmlParser.HtmlHorizontalRule
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Name |  | 


## `HtmlImageElement`

```csharp
public class InnerLibs.HtmlParser.HtmlImageElement
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Alt |  | 
| `String` | Name |  | 
| `String` | Src |  | 


## `HtmlInputElement`

```csharp
public class InnerLibs.HtmlParser.HtmlInputElement
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Name |  | 
| `String` | PlaceHolder | Placeholder of Input | 
| `Boolean` | ReadOnly |  | 
| `HtmlInputElementType` | Type | Type of Input | 
| `String` | Value | Value of Input | 


## `HtmlListElement`

```csharp
public class InnerLibs.HtmlParser.HtmlListElement
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsOrdenedList |  | 
| `String` | Name | Returns the name of element (OL or UL) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`String` Text) | Add a LI to this list | 
| `void` | Add(`HtmlNode[]` Content) | Add a LI to this list | 


## `HtmlNode`

The HtmlNode is the base for all objects that may appear in HTML. Currently, this  implemention only supports HtmlText and HtmlElement node types.
```csharp
public abstract class InnerLibs.HtmlParser.HtmlNode

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ElementRepresentation |  | 
| `HtmlNode` | FirstChild | This will return the first child node. If there are no children, this will return null. | 
| `String` | HTML | This will return the full HTML to represent this node (and all child nodes). | 
| `Int32` | Index | This will return the index position within the parent's nodes that this one resides. If  this is not in a collection, this will return -1. | 
| `Boolean` | IsChild | This will return true if this is a child node (has a parent). | 
| `Boolean` | IsParent |  | 
| `Boolean` | IsRoot | This will return true if this is a root node (has no parent). | 
| `Dictionary<String, Object>` | JsonRepresentation | Return as | 
| `HtmlNode` | LastChild | This will return the last child node. If there are no children, this will return null. | 
| `HtmlNode` | Next | This will return the next sibling node. If this is the last one, it will return null. | 
| `HtmlElement` | Parent | This will return the parent of this node, or null if there is none. | 
| `HtmlNode` | Previous | This will return the previous sibling node. If this is the first one, it will return null. | 
| `String` | XHTML | This will return the full XHTML to represent this node (and all child nodes) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlNode` | AppendTo(`HtmlElement` Element, `Boolean` Copy = False) |  | 
| `HtmlElement` | AsElement() |  | 
| `ElementType` | AsElement() |  | 
| `HtmlText` | AsText() |  | 
| `Boolean` | Censor(`Char` CensorChar, `String[]` BadWords) |  | 
| `HtmlElement` | Closest(`String` CssSelector) | Returns the most closest parent matching the css selector | 
| `void` | FixText() |  | 
| `HtmlNode` | GetCommonAncestor(`HtmlNode` node) | This will return the ancstor that is common to this node and the one specified. | 
| `Boolean` | IsAncestorOf(`HtmlNode` node) | This will return true if the node passed is one of the children or grandchildren of this node. | 
| `Boolean` | IsDescendentOf(`HtmlNode` node) | This will return true if the node passed is a descendent of this node. | 
| `Boolean` | IsElement() |  | 
| `Boolean` | IsElement() |  | 
| `Boolean` | IsText() |  | 
| `void` | Move(`HtmlElement` Destination, `Int32` Index = 0) | Transfer the element to another element | 
| `void` | Remove() | This will remove this node and all child nodes from the tree. If this is a root node,  this operation will do nothing. | 
| `String` | ToJSON() | Return a Json representation of this element | 
| `HtmlElement` | TopParent() | Returns the most top parent of this node, or sef if parent is null | 
| `String` | ToString() | This will render the node as it would appear in HTML. | 


## `HtmlNodeCollection`

This object represents a collection of HtmlNodes, which can be either HtmlText or HtmlElement  objects. The order in which the nodes occur directly corresponds to the order in which they  appear in the original HTML document.
```csharp
public class InnerLibs.HtmlParser.HtmlNodeCollection
    : List<HtmlNode>, IList<HtmlNode>, ICollection<HtmlNode>, IEnumerable<HtmlNode>, IEnumerable, IList, ICollection, IReadOnlyList<HtmlNode>, IReadOnlyCollection<HtmlNode>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlNodeCollection` | Item | Return elements thats match the current CSS selector | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`HtmlNode` Node) | Add a Node to colleciton | 
| `void` | Add(`String` Html, `Int32` Index = 0) | Add a Node to colleciton | 
| `void` | Add(`HtmlGenericControl` Control) | Add a Node to colleciton | 
| `void` | Add(`HtmlNode[]` Nodes) | Add a Node to colleciton | 
| `HtmlNodeCollection` | Do(`Action<HtmlNode>` action) | Perform an action on each node and returns the same list | 
| `HtmlNodeCollection` | FindElements(`Func<NodeType, Boolean>` predicate, `Boolean` SearchChildren = True) | This will search though this collection of nodes for all elements with matchs the predicate. | 
| `HtmlNodeCollection` | FindElements() | This will search though this collection of nodes for all elements with matchs the predicate. | 
| `IEnumerable<HtmlElement>` | GetElements() | Return only `InnerLibs.HtmlParser.HtmlElement` from this `InnerLibs.HtmlParser.HtmlNodeCollection` | 
| `HtmlNodeCollection` | GetElementsByAttributeName(`String` AttributeName, `Boolean` SearchChildren = True) | This will search though this collection of nodes for all elements with the an attribute  with the given name. | 
| `HtmlNodeCollection` | GetElementsByAttributeNameValue(`String` AttributeName, `String` AttributeValue, `Boolean` SearchChildren = True) | This will search though this collection of nodes for all elements with the an attribute  with the given name and value. | 
| `HtmlNodeCollection` | GetElementsByTagName(`String` Name, `Boolean` SearchChildren = True) | This will search though this collection of nodes for all elements with the specified  name. If you want to search the subnodes recursively, you should pass True as the  parameter in SearchChildren. This search is guaranteed to return nodes in the order in  which they are found in the document. | 
| `void` | Insert(`Int32` Index, `HtmlNode[]` Nodes) | Insert a element in specific index | 
| `void` | Insert(`Int32` Index, `String` Nodes) | Insert a element in specific index | 
| `void` | Insert(`Int32` Index, `HtmlNodeCollection` Nodes) | Insert a element in specific index | 
| `void` | ReplaceElement(`HtmlNode` Element, `IEnumerable<HtmlNode>` Items) | Replace a element into another set of elements | 
| `void` | ReplaceElement(`HtmlNode` Element, `String` Html) | Replace a element into another set of elements | 
| `String` | ToString() | Retuns all html text from this collection | 


## `HtmlOptionElement`

```csharp
public class InnerLibs.HtmlParser.HtmlOptionElement
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Group |  | 
| `String` | Text |  | 
| `String` | Value |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ListItem` | AsListItem() |  | 


## `HtmlSelectElement`

```csharp
public class InnerLibs.HtmlParser.HtmlSelectElement
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | Groups |  | 
| `String` | Name | Returns the name of element (OL or UL) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | AddOption(`HtmlOptionElement` Option) | Add a option to this list | 
| `void` | Organize() | Redefines the node elements | 


## `HtmlText`

The HtmlText node represents a simple piece of text from the document.
```csharp
public class InnerLibs.HtmlParser.HtmlText
    : HtmlNode

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ElementRepresentation | This will return the text for outputting inside an HTML document. | 
| `String` | HTML | This will return the HTML to represent this text object. | 
| `String` | Text | This is the text associated with this node. | 
| `String` | XHTML | This will return the XHTML to represent this text object. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Censor(`Char` CensorChar, `String[]` BadWords) | ReplaceFrom Badwords in text. | 
| `void` | FixText() | Fix the punctuation, white spaces and captalization of text | 
| `String` | ToString() | This will return the text for outputting inside an HTML document. | 


## `HtmlTimeElement`

```csharp
public class InnerLibs.HtmlParser.HtmlTimeElement
    : HtmlElement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Nullable<DateTime>` | DateTime |  | 
| `String` | Name |  | 


## `MentionParser`

```csharp
public class InnerLibs.HtmlParser.MentionParser

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlAnchorElement` | CreateAnchor(this `String` URL, `String` Target = _blank, `String` Title = ) | Cria um elemento de Ancora (a) a partir de uma string com URL. O titulo é obtido  automaticamente da url quando possivel. Se a string não for uma URL válida uma ancora com  o proprio texto é criada. | 
| `String` | ParseEmoji(this `String` Text, `Func<String, String>` Method = null) | Localiza emojis no texto e automaticamente executa uma função de replace para cada emoji encontrado | 
| `String` | ParseHashtag(this `String` Text, `Func<String, String>` Method = null) | Localiza hashtags no texto e automaticamente executa uma função de replace para cada  hashtag encontrada | 
| `String` | ParseMentionByChar(this `String` Text, `String` MatchChar, `Func<String, String>` Method = null) | Localiza menções a usuários no texto a partir de um caractere e automaticamente executa uma função de replace para  cada menção encontrada | 
| `String` | ParseURL(this `String` Text, `Func<String, String>` Method = null) | Localiza URLs no texto e automaticamente executa uma função de replace para cada URL encontrada | 
| `String` | ParseUsername(this `String` Text, `Func<String, String>` Method = null) | Localiza menções a usuários no texto e automaticamente executa uma função de replace para  cada hashtag encontrada | 


## `PseudoClass`

```csharp
public abstract class InnerLibs.HtmlParser.PseudoClass

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlNodeCollection` | Filter(`HtmlNodeCollection` nodes, `String` parameter) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PseudoClass` | GetPseudoClass(`String` pseudoClass) |  | 


## `PseudoClassNameAttribute`

```csharp
public class InnerLibs.HtmlParser.PseudoClassNameAttribute
    : Attribute, _Attribute

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | FunctionName |  | 


## `Token`

```csharp
public class InnerLibs.HtmlParser.Token

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Filter |  | 
| `IList<Token>` | SubTokens |  | 


## `Tokenizer`

```csharp
public class InnerLibs.HtmlParser.Tokenizer

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<Token>` | GetTokens(`String` cssFilter) |  | 


